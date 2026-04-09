using System;

namespace FertileNotify.Application.Services.Notifications
{
    public class SystemNotificationService: ISystemNotificationService
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<SystemNotificationService> _logger;
        private readonly ISystemNotificationRepository _notificationRepository;

        public SystemNotificationService(
            ISubscriberRepository subscriberRepository,
            IEmailService emailService,
            ILogger<SystemNotificationService> logger,
            ISystemNotificationRepository notificationRepository)
        {
            _subscriberRepository = subscriberRepository;
            _emailService = emailService;
            _logger = logger;
            _notificationRepository = notificationRepository;
        }

        public async Task SendAsync(Guid subscriberId, string title, string message, bool sendEmail = false)
        {
            var notification = new SystemNotification(
                subscriberId,
                title,
                message
            );
            await _notificationRepository.AddAsync(notification);

            if (sendEmail)
            {
                var subscriber = await _subscriberRepository.GetByIdAsync(subscriberId);
                if (subscriber != null)
                {
                    await _emailService.SendEmailAsync(subscriber.Email, title, message);
                }
            }

            _logger.LogInformation("Sending system notification to subscriber {SubscriberId}: {Title} - {Message}", subscriberId, title, message);
        }
    }
}
