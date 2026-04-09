namespace FertileNotify.Application.UseCases.NotificationComplaint
{
    public class NotificationComplaintHandler : ICommandHandler<NotificationComplaintCommand>
    {
        private readonly INotificationComplaintRepository _complaintRepository;
        private readonly ILogger<NotificationComplaintHandler> _logger;
        private readonly ISystemNotificationService _notificationService;

        public NotificationComplaintHandler(
            INotificationComplaintRepository complaintRepository,
            ILogger<NotificationComplaintHandler> logger,
            ISystemNotificationService notificationService)
        {
            _complaintRepository = complaintRepository;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<Unit> Handle(NotificationComplaintCommand command, CancellationToken cancellationToken)
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

            await _notificationService.SendAsync(
                command.SubscriberId,
                "There is a Complaint",
                $"One of your recipients sent us the notification {command.Subject} -- {command.Body}. Our team has launched an investigation. " +
                "We will get back to you as soon as possible.",
                sendEmail: true
            );

            return Unit.Value;
        }
    }
}
