using System;
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
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<OtpService>> _loggerMock;
        private readonly OtpService _otpService;

        public OtpServiceTests()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<OtpService>>();
            _otpService = new OtpService(_cacheMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GenerateOtpAsync_ShouldReturnOtpOfCorrectLength()
        {
            // Arrange
            var subscriberId = Guid.NewGuid();
            var length = 6;
            _configurationMock.Setup(x => x.GetSection("OTPSettings:Length").Value).Returns(length.ToString());

            // Act
            var otp = await _otpService.GenerateOtpAsync(subscriberId);

            // Assert
            otp.Should().HaveLength(length);
            int.TryParse(otp, out _).Should().BeTrue();
        }

        [Fact]
        public async Task VerifyOtpAsync_ShouldReturnTrue_WhenOtpIsValid()
        {
            // Arrange
            var subscriberId = Guid.NewGuid();
            var otp = "123456";
            var key = $"otp_{subscriberId}";
            var otpBytes = System.Text.Encoding.UTF8.GetBytes(otp);
            _cacheMock.Setup(x => x.GetAsync(key, default)).ReturnsAsync(otpBytes);

            // Act
            var result = await _otpService.VerifyOtpAsync(subscriberId, otp);

            // Assert
            result.Should().BeTrue();
            _cacheMock.Verify(x => x.RemoveAsync(key, default), Times.Once);
        }

        [Fact]
        public async Task VerifyOtpAsync_ShouldReturnFalse_WhenOtpIsInvalid()
        {
            // Arrange
            var subscriberId = Guid.NewGuid();
            var otp = "123456";
            var key = $"otp_{subscriberId}";
            var cachedOtpBytes = System.Text.Encoding.UTF8.GetBytes("654321");
            _cacheMock.Setup(x => x.GetAsync(key, default)).ReturnsAsync(cachedOtpBytes);

            // Act
            var result = await _otpService.VerifyOtpAsync(subscriberId, otp);

            // Assert
            result.Should().BeFalse();
            _cacheMock.Verify(x => x.RemoveAsync(key, default), Times.Never);
        }
    }
}
