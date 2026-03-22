using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
    }
}
