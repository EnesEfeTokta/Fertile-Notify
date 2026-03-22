using System.Net;
using System.Net.Http.Json;
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
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
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

            if (!db.Subscribers.Any(s => s.Email.Value == "test@example.com"))
            {
                var subscriber = new Subscriber(
                    CompanyName.Create("Test Company"),
                    Password.Create("Password123!"),
                    EmailAddress.Create("test@example.com"),
                    PhoneNumber.Create("+1234567890"));

                db.Subscribers.Add(subscriber);
                db.Subscriptions.Add(Subscription.Create(subscriber.Id, FertileNotify.Domain.Enums.SubscriptionPlan.Free));
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Login_ShouldReturnNotFound_WhenSubscriberDoesNotExist()
        {
            var request = new UserLoginRequest
            {
                Email = "nonexistenttest@example.com",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_AndTriggerOtp_WhenCredentialsAreValid()
        {
            var request = new UserLoginRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task VerifyCode_ShouldReturnUnauthorized_WithInvalidOtp()
        {
            var request = new OtpRequest
            {
                Email = "test@example.com",
                OtpCode = "000000"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/verify-code", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task VerifyCode_ShouldReturnTokens_WithValidOtp()
        {
            var request = new OtpRequest
            {
                Email = "test@example.com",
                OtpCode = FakeOtpService.FixedOtp
            };

            var response = await _client.PostAsJsonAsync("/api/auth/verify-code", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Data!.AccessToken.Should().NotBeNullOrEmpty();
            result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ForgotPassword_ShouldReturnOk()
        {
            var request = new ForgotPasswordRequest
            {
                Email = "test@example.com"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/forgot-password", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens_WhenValid()
        {
            // 1. Login to get a valid refresh token
            var verifyRequest = new OtpRequest
            {
                Email = "test@example.com",
                OtpCode = FakeOtpService.FixedOtp
            };
            var verifyResponse = await _client.PostAsJsonAsync("/api/auth/verify-code", verifyRequest);
            var loginResult = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();

            // 2. Refresh Token
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = loginResult!.Data.RefreshToken
            };

            var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var refreshResult = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
            refreshResult.Should().NotBeNull();
            refreshResult!.Data!.AccessToken.Should().NotBeNullOrEmpty();
            refreshResult.Data.RefreshToken.Should().NotBeNullOrEmpty();
            refreshResult.Data.RefreshToken.Should().NotBe(loginResult.Data.RefreshToken); // Should be a new token
        }
    }
}
