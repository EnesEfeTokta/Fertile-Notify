using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.NotificationComplaint
{
    public class NotificationComplaintHandler
    {
        private readonly IEmailService _emailService;
        private readonly INotificationComplaintRepository _notificationComplaintRepository;
        private readonly ILogger<NotificationComplaintHandler> _logger;
        private readonly ISubscriberRepository _subscriberRepository;

        public NotificationComplaintHandler(
            IEmailService emailService, 
            INotificationComplaintRepository notificationComplaintRepository, 
            ILogger<NotificationComplaintHandler> logger,
            ISubscriberRepository subscriberRepository
        )
        {
            _emailService = emailService;
            _notificationComplaintRepository = notificationComplaintRepository;
            _logger = logger;
            _subscriberRepository = subscriberRepository;
        }

        public async Task HandleAsync(NotificationComplaintCommand command)
        {
            try
            {
                var complaint = Domain.Entities.NotificationComplaint.Create(
                    command.SubscriberId,
                    EmailAddress.Create(command.ReporterEmail),
                    Enum.Parse<Domain.Enums.ComplaintType>(command.Reason, ignoreCase: true),
                    command.Description,
                    command.NotificationSubject,
                    command.NotificationBody
                );
                await _notificationComplaintRepository.SaveAsync(complaint);
                _logger.LogInformation("Notification complaint recorded: {ComplaintId}", complaint.Id);

                var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId);

                await _emailService.SendEmailAsync(
                    subscriber!.Email,
                    "New Notification Complaint", 
                    $"A new complaint has been filed by {command.ReporterEmail} for SubscriberId: {command.SubscriberId}. Reason: {command.Reason}. Description: {command.Description}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling notification complaint for SubscriberId: {SubscriberId}", command.SubscriberId);
                throw;
            }
        }
    }
}
