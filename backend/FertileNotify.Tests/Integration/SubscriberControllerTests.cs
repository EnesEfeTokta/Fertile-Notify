using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.DTOs;
using FertileNotify.Domain.Entities;
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
                    Password.Create("Password123!"),
                    EmailAddress.Create("subtest@example.com"),
                    PhoneNumber.Create("+1234567891"));

                db.Subscribers.Add(subscriber);
                db.Subscriptions.Add(Subscription.Create(subscriber.Id, FertileNotify.Domain.Enums.SubscriptionPlan.Free));
                db.SaveChanges();
            }
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
    }
}
