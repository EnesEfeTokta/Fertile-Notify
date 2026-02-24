using FertileNotify.Domain.ValueObjects;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class EmailAddressTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("test")]
        [InlineData("test@")]
        [InlineData("@test.com")]
        [InlineData("test@test")]
        public void Create_Should_ThrowException_When_Format_Is_Invalid(string invalidEmail)
        {
            var action = () => EmailAddress.Create(invalidEmail);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_Should_Trim_And_Lowercase_Email()
        {
            var email = EmailAddress.Create("  Test@Company.COM  ");

            email.Value.Should().Be("test@company.com");
        }
    }
}