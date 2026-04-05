namespace FertileNotify.Application.UseCases.Templates
{
    public class CreateOrUpdateCustomTemplateCommand : ICommand
    {
        public Guid SubscriberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class CreateOrUpdateCustomTemplateHandler : ICommandHandler<CreateOrUpdateCustomTemplateCommand>
    {
        private readonly ITemplateRepository _templateRepository;

        public CreateOrUpdateCustomTemplateHandler(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<Unit> Handle(CreateOrUpdateCustomTemplateCommand command, CancellationToken cancellationToken)
        {
            var eventType = EventType.From(command.EventType);
            var channel = NotificationChannel.From(command.Channel);

            var existingTemplate = await _templateRepository.GetCustomTemplateAsync(eventType, channel, command.SubscriberId);

            if (existingTemplate != null)
            {
                existingTemplate.Update(command.Name, command.Description, new NotificationContent(command.Subject, command.Body));
                await _templateRepository.SaveAsync();
            }
            else
            {
                var newTemplate = NotificationTemplate.CreateCustom(
                    command.SubscriberId,
                    command.Name,
                    command.Description,
                    eventType,
                    channel,
                    new NotificationContent(command.Subject, command.Body)
                );
                await _templateRepository.AddAsync(newTemplate);
            }

            return Unit.Value;
        }
    }
}