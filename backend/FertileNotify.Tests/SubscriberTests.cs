using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class SubscriberTests
    {
        [Fact]
        public void EnableChannel_Should_ThrowException_When_Plan_Does_Not_Support_Channel()
        {
            var subscriber = new Subscriber(
                CompanyName.Create("Test LTC"),
                Password.Create("StrongP@ssw0rd"),
                EmailAddress.Create("test@test.com"),
                PhoneNumber.Create("+123-456-78-90")
            );

            var plan = SubscriptionPlan.Free;

            var action = () => subscriber.EnableChannel(NotificationChannel.SMS, plan);
            action.Should().Throw<BusinessRuleException>().WithMessage("Your plan (Free) does not support the sms channel.");
        }

        [Fact]
        public void EnableChannel_Should_Add_Channel_When_Plan_Supports_It()
        {
            var subscriber = new Subscriber(
                CompanyName.Create("Test LTC"),
                Password.Create("StrongP@ssw0rd"),
                EmailAddress.Create("test@test.com"),
                null
            );

            var plan = SubscriptionPlan.Free;

            subscriber.EnableChannel(NotificationChannel.Console, plan);
            subscriber.ActiveChannels.Should().Contain(NotificationChannel.Console);
        }

        [Fact]
        public void EnableChannel_Should_ThrowException_When_SMS_Added_Without_PhoneNumber()
        {
            var subscriber = new Subscriber(
                CompanyName.Create("Test LTC"),
                Password.Create("StrongP@ssw0rd"),
                EmailAddress.Create("test@test.com"),
                null
            );

            var plan = SubscriptionPlan.Pro;

            var action = () => subscriber.EnableChannel(NotificationChannel.SMS, plan);
            action.Should().Throw<BusinessRuleException>().WithMessage("Phone number is required to enable SMS channel.");
        }
    }
}
