using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.DTOs;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FertileNotify.Infrastructure.Persistence;
using FertileNotify.Tests.Integration.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FertileNotify.Tests.Integration
{

    public class SubscriberControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public SubscriberControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            if (!db.Subscribers.Any(s => s.Email.Value == "subtest@example.com"))
            {
                var subscriber = new Subscriber(
                    CompanyName.Create("Sub Test Company"),
                    "Sub test company description",
                    CustomUrl.Create("https://example.com/logo-sub.png"),
                    CustomUrl.Create("https://subtest.example.com"),
                    "Ankara",
                    Password.Create("Password123!"),
                    EmailAddress.Create("subtest@example.com"),
                    PhoneNumber.Create("+1234567891"));

                db.Subscribers.Add(subscriber);
                db.Subscriptions.Add(Subscription.Create(subscriber.Id, FertileNotify.Domain.Enums.SubscriptionPlan.Free));
                db.SaveChanges();
            }
        }

        private AutomationWorkflow EnsureWorkflowSeeded(string eventTrigger = "user_signup")
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            var subscriber = db.Subscribers.First(s => s.Email.Value == "subtest@example.com");
            var existing = db.AutomationWorkflows.FirstOrDefault(w => w.SubscriberId == subscriber.Id && w.EventTrigger == eventTrigger);
            if (existing != null)
                return existing;

            var workflow = new AutomationWorkflow(
                subscriber.Id,
                "Workflow Trigger Test",
                "Integration test workflow",
                NotificationContent.Create("Workflow Subject", "Workflow Body"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                eventTrigger,
                string.Empty,
                new List<string> { "user@example.com" });

            db.AutomationWorkflows.Add(workflow);
            db.SaveChanges();
            return workflow;
        }

        private string EnsureWorkflowApiKeySeeded()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            var subscriber = db.Subscribers.First(s => s.Email.Value == "subtest@example.com");
            const string rawKey = "fn_workflow_trigger_key_123456789";

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
            var hash = Convert.ToBase64String(hashBytes);

            if (!db.ApiKeys.Any(k => k.SubscriberId == subscriber.Id && k.KeyHash == hash))
            {
                db.ApiKeys.Add(new ApiKey(subscriber.Id, hash, rawKey.Substring(0, 7), "Workflow API Key", "workflow:trigger"));
                db.SaveChanges();
            }

            return rawKey;
        }

        private async Task<string> GetAccessTokenAsync(string email)
        {
            // 1. Request Login (OTP trigger)
            await _client.PostAsJsonAsync("/api/auth/login", new UserLoginRequest { Email = email, Password = "Password123!" });

            // 2. Verify OTP
            var verifyResponse = await _client.PostAsJsonAsync("/api/auth/verify-code", new OtpRequest { Email = email, OtpCode = FakeOtpService.FixedOtp });
            var result = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
            return result!.Data!.AccessToken;
        }

        [Fact]
        public async Task GetMyProfile_ShouldReturnOk_WhenAuthenticated()
        {
            // Arrange
            var token = await GetAccessTokenAsync("subtest@example.com");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/subscribers/me");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<SubscriberDto>>();
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Data!.Email.Should().Be("subtest@example.com");
        }

        [Fact]
        public async Task Register_ShouldReturnOk()
        {
            // Arrange
            var request = new RegisterSubscriberRequest
            {
                CompanyName = "New Project",
                CompanyDescription = "New Project Company Description",
                LogoUrl = "https://newproject.example.com/logo.png",
                WebsiteUrl = "https://newproject.example.com",
                Location = "Izmir",
                Email = "newproject@example.com",
                Password = "SecurePassword123!",
                PhoneNumber = "+905554443322",
                Plan = "Free"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/subscribers/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExportData_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Act
            var response = await _client.GetAsync("/api/subscribers/export-data");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ExportData_ShouldReturnDownloadableJson_WhenAuthenticated()
        {
            // Arrange
            var token = await GetAccessTokenAsync("subtest@example.com");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/subscribers/export-data");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
            response.Content.Headers.ContentDisposition.Should().NotBeNull();
            response.Content.Headers.ContentDisposition!.DispositionType.Should().Be("attachment");
            response.Content.Headers.ContentDisposition.FileName.Should().NotBeNull();
            response.Content.Headers.ContentDisposition.FileName!.Should().Contain("fertilenotify-export-");
            response.Content.Headers.ContentDisposition.FileName!.Should().EndWith(".json");

            var payload = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;

            root.TryGetProperty("DataOwnerSubscriberId", out var ownerId).Should().BeTrue();
            ownerId.GetGuid().Should().NotBe(Guid.Empty);

            root.TryGetProperty("ExportedAtUtc", out _).Should().BeTrue();
            root.TryGetProperty("Subscriber", out var subscriber).Should().BeTrue();
            subscriber.TryGetProperty("Email", out var email).Should().BeTrue();
            email.GetString().Should().Be("subtest@example.com");

            root.TryGetProperty("ApiKeys", out var apiKeys).Should().BeTrue();
            apiKeys.ValueKind.Should().Be(JsonValueKind.Array);

            root.TryGetProperty("NotificationLogs", out var logs).Should().BeTrue();
            logs.ValueKind.Should().Be(JsonValueKind.Array);

            root.TryGetProperty("NotificationTemplates", out var templates).Should().BeTrue();
            templates.ValueKind.Should().Be(JsonValueKind.Array);

            root.TryGetProperty("ChannelConfigurations", out var channelConfigurations).Should().BeTrue();
            channelConfigurations.ValueKind.Should().Be(JsonValueKind.Array);

            root.TryGetProperty("WorkflowNotifications", out var workflows).Should().BeTrue();
            workflows.ValueKind.Should().Be(JsonValueKind.Array);

            root.TryGetProperty("BlacklistEntries", out var blacklistEntries).Should().BeTrue();
            blacklistEntries.ValueKind.Should().Be(JsonValueKind.Array);

            root.TryGetProperty("NotificationComplaints", out var complaints).Should().BeTrue();
            complaints.ValueKind.Should().Be(JsonValueKind.Array);
        }

        [Fact]
        public async Task ApiKeyLifecycle_ShouldSupportCreateUpdateStatusAndDelete()
        {
            // Arrange
            var token = await GetAccessTokenAsync("subtest@example.com");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResponse = await _client.PostAsJsonAsync("/api/subscribers/api-keys", new CreateApiKeyRequest
            {
                Name = "Lifecycle Key"
            });

            // Assert create
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createPayload = await createResponse.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>();
            createPayload.Should().NotBeNull();
            createPayload!.Success.Should().BeTrue();

            var listAfterCreateResponse = await _client.GetAsync("/api/subscribers/api-keys");
            listAfterCreateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var listAfterCreatePayload = await listAfterCreateResponse.Content.ReadFromJsonAsync<ApiResponse<List<ApiKeyDto>>>();
            listAfterCreatePayload.Should().NotBeNull();
            listAfterCreatePayload!.Success.Should().BeTrue();

            var createdKey = listAfterCreatePayload.Data!
                .FirstOrDefault(k => k.Name == "Lifecycle Key");

            createdKey.Should().NotBeNull();

            // Update scopes
            var updateScopesResponse = await _client.PatchAsJsonAsync(
                $"/api/subscribers/api-keys/{createdKey!.Id}/scopes",
                new UpdateApiKeyScopesRequest { Scopes = "notifications:read,notifications:write" });

            updateScopesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update status
            var updateStatusResponse = await _client.PatchAsJsonAsync(
                $"/api/subscribers/api-keys/{createdKey.Id}/status",
                new UpdateApiKeyStatusRequest { IsActive = false });

            updateStatusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var listAfterUpdatesResponse = await _client.GetAsync("/api/subscribers/api-keys");
            listAfterUpdatesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var listAfterUpdatesPayload = await listAfterUpdatesResponse.Content.ReadFromJsonAsync<ApiResponse<List<ApiKeyDto>>>();
            var updatedKey = listAfterUpdatesPayload!.Data!
                .FirstOrDefault(k => k.Id == createdKey.Id);

            updatedKey.Should().NotBeNull();
            updatedKey!.Scopes.Should().Be("notifications:read,notifications:write");
            updatedKey.IsActive.Should().BeFalse();

            // Delete
            var deleteResponse = await _client.DeleteAsync($"/api/subscribers/api-keys/{createdKey.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var listAfterDeleteResponse = await _client.GetAsync("/api/subscribers/api-keys");
            listAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var listAfterDeletePayload = await listAfterDeleteResponse.Content.ReadFromJsonAsync<ApiResponse<List<ApiKeyDto>>>();
            listAfterDeletePayload!.Data!
                .Any(k => k.Id == createdKey.Id)
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task WorkflowSend_ShouldAllowJwtAndApiKeyAccess()
        {
            // Arrange
            var workflow = EnsureWorkflowSeeded();
            var token = await GetAccessTokenAsync("subtest@example.com");

            // JWT path
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jwtResponse = await _client.PostAsync($"/api/notifications/workflow/send/{workflow.EventTrigger}", null);

            jwtResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

            // API key path
            var apiKeyClient = _factory.CreateClient();
            apiKeyClient.DefaultRequestHeaders.Add("FN-Api-Key", EnsureWorkflowApiKeySeeded());

            var apiKeyResponse = await apiKeyClient.PostAsync($"/api/notifications/workflow/send/{workflow.EventTrigger}", null);

            apiKeyResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }
    }
}
