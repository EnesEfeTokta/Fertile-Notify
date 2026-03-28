using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FertileNotify.Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FertileNotify.Tests.Infrastructure
{
    public class OtpServiceTests
    {
        private IConfiguration GetConfiguration(string length = "6", string expiry = "5")
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"OTPSettings:Length", length},
                {"OTPSettings:ExpiryInMinutes", expiry}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        [Fact]
        public async Task GenerateOtpAsync_ShouldReturnOtpOfCorrectLength()
        {
            // Arrange
            var cacheMock = new Mock<IDistributedCache>();
            var loggerMock = new Mock<ILogger<OtpService>>();
            var configuration = GetConfiguration();
            var otpService = new OtpService(cacheMock.Object, configuration, loggerMock.Object);
            var subscriberId = Guid.NewGuid();

            // Act
            var otp = await otpService.GenerateOtpAsync(subscriberId);

            // Assert
            otp.Should().HaveLength(6);
            int.TryParse(otp, out _).Should().BeTrue();
        }

        [Fact]
        public async Task VerifyOtpAsync_ShouldReturnTrue_WhenOtpIsValid()
        {
            // Arrange
            var cacheMock = new Mock<IDistributedCache>();
            var loggerMock = new Mock<ILogger<OtpService>>();
            var configuration = GetConfiguration();
            var otpService = new OtpService(cacheMock.Object, configuration, loggerMock.Object);

            var subscriberId = Guid.NewGuid();
            var otp = "123456";
            var key = $"otp_{subscriberId}";
            var otpBytes = System.Text.Encoding.UTF8.GetBytes(otp);
            cacheMock.Setup(x => x.GetAsync(key, default)).ReturnsAsync(otpBytes);

            // Act
            var result = await otpService.VerifyOtpAsync(subscriberId, otp);

            // Assert
            result.Should().BeTrue();
            cacheMock.Verify(x => x.RemoveAsync(key, default), Times.Once);
        }

        [Fact]
        public async Task VerifyOtpAsync_ShouldReturnFalse_WhenOtpIsInvalid()
        {
            // Arrange
            var cacheMock = new Mock<IDistributedCache>();
            var loggerMock = new Mock<ILogger<OtpService>>();
            var configuration = GetConfiguration();
            var otpService = new OtpService(cacheMock.Object, configuration, loggerMock.Object);

            var subscriberId = Guid.NewGuid();
            var otp = "123456";
            var key = $"otp_{subscriberId}";
            var cachedOtpBytes = System.Text.Encoding.UTF8.GetBytes("654321");
            cacheMock.Setup(x => x.GetAsync(key, default)).ReturnsAsync(cachedOtpBytes);

            // Act
            var result = await otpService.VerifyOtpAsync(subscriberId, otp);

            // Assert
            result.Should().BeFalse();
            cacheMock.Verify(x => x.RemoveAsync(key, default), Times.Never);
        }
    }
}
