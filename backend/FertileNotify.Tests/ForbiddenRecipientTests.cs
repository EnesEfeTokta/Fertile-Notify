using System;
using System.Collections.Generic;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class ForbiddenRecipientTests
    {
        [Fact]
        public void Deactivate_Should_Set_IsActive_False()
        {
            var entry = new ForbiddenRecipient(Guid.NewGuid(), "test@test.com", new List<NotificationChannel>());
            
            entry.Deactivate();

            entry.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Activate_Should_Set_IsActive_True()
        {
            var entry = new ForbiddenRecipient(Guid.NewGuid(), "test@test.com", new List<NotificationChannel>());
            entry.Deactivate();
            
            entry.Activate();

            entry.IsActive.Should().BeTrue();
        }

        [Fact]
        public void UpdateChannels_Should_Replace_Channel_List()
        {
            var entry = new ForbiddenRecipient(Guid.NewGuid(), "test@test.com", new List<NotificationChannel> { NotificationChannel.Email });
            var newChannels = new List<NotificationChannel> { NotificationChannel.SMS, NotificationChannel.WebPush };

            entry.UpdateChannels(newChannels);

            entry.UnwantedChannels.Should().HaveCount(2);
            entry.UnwantedChannels.Should().Contain(NotificationChannel.SMS);
            entry.UnwantedChannels.Should().Contain(NotificationChannel.WebPush);
            entry.UnwantedChannels.Should().NotContain(NotificationChannel.Email);
        }
    }
}
