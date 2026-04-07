namespace FertileNotify.Application.UseCases.NotificationComplaint
{
    public class NotificationComplaintHandler : ICommandHandler<NotificationComplaintCommand>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly INotificationComplaintRepository _complaintRepository;
        private readonly ILogger<NotificationComplaintHandler> _logger;
        private readonly IEmailService _emailService;

        public NotificationComplaintHandler(
            ISubscriberRepository subscriberRepository,
            INotificationComplaintRepository complaintRepository,
            ILogger<NotificationComplaintHandler> logger,
            IEmailService emailService)
        {
            _subscriberRepository = subscriberRepository;
            _complaintRepository = complaintRepository;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(NotificationComplaintCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var complaint = Domain.Entities.NotificationComplaint.Create(
                    command.SubscriberId,
                    EmailAddress.Create(command.ReporterEmail),
                    Enum.Parse<ComplaintType>(command.Reason, ignoreCase: true),
                    command.Description,
                    new NotificationContent(command.Subject, command.Body)
                );
                await _complaintRepository.SaveAsync(complaint);
                _logger.LogInformation("Notification complaint recorded: {ComplaintId}", complaint.Id);

                var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId);

                await _emailService.SendEmailAsync(
                    subscriber!.Email,
                    "New Notification Complaint", 
                    $"A new complaint has been filed by {command.ReporterEmail} for SubscriberId: {command.SubscriberId}. Reason: {command.Reason}. Description: {command.Description}"
                );
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling notification complaint for SubscriberId: {SubscriberId}", command.SubscriberId);
                throw;
            }
        }
    }
}
