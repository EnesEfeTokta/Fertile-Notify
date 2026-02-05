using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FertileNotify.Tests
{
    public class PasswordTests
    {
        [Theory]
        [InlineData("123")]
        [InlineData("abcdefgh")]
        [InlineData("ABCDEFGH")]
        [InlineData("Abcdefgh")]
        public void Create_Should_ThrowException_When_Password_Is_Weak(string weakPassword)
        {
            var action = () => Password.Create(weakPassword);
            action.Should().Throw<BusinessRuleException>();
        }

        [Fact]
        public void Create_Should_Return_Password_When_Strong()
        {
            var password = Password.Create("StrongP@ss1");

            password.Should().NotBeNull();
            password.Hash.Should().NotBeNullOrEmpty();
            password.Hash.Should().NotBe("StrongP@ss1");
        }

        [Fact]
        public void Verify_Should_Return_True_For_Correct_Password()
        {
            var plain = "StrongP@ss1";
            var password = Password.Create(plain);

            var result = password.Verify(plain);

            result.Should().BeTrue();
        }
    }
}
