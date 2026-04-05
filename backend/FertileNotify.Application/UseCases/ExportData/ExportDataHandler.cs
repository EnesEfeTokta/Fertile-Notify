namespace FertileNotify.Application.UseCases.ExportData
{
    public class ExportDataHandler : IQueryHandler<ExportDataQuery, ExportDataDto>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly INotificationLogRepository _notificationLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IAutomationRepository _automationRepository;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly INotificationComplaintRepository _notificationComplaintRepository;

        public ExportDataHandler(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            IApiKeyRepository apiKeyRepository,
            ISubscriberChannelRepository subscriberChannelRepository,
            INotificationLogRepository notificationLogRepository,
            ITemplateRepository templateRepository,
            IAutomationRepository automationRepository,
            IBlacklistRepository blacklistRepository,
            INotificationComplaintRepository notificationComplaintRepository)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _apiKeyRepository = apiKeyRepository;
            _subscriberChannelRepository = subscriberChannelRepository;
            _notificationLogRepository = notificationLogRepository;
            _templateRepository = templateRepository;
            _automationRepository = automationRepository;
            _blacklistRepository = blacklistRepository;
            _notificationComplaintRepository = notificationComplaintRepository;
        }

        public async Task<ExportDataDto> Handle(ExportDataQuery command, CancellationToken ct)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscription not found.");

            var apiKeysTask = _apiKeyRepository.GetBySubscriberIdAsync(command.SubscriberId);
            var channelConfigurationsTask = _subscriberChannelRepository.GetAllSettingsAsync(command.SubscriberId);
            var notificationLogsTask = _notificationLogRepository.GetAllBySubscriberIdAsync(command.SubscriberId);
            var templatesTask = _templateRepository.GetAllTemplatesAsync(command.SubscriberId);
            var workflowsTask = _automationRepository.GetBySubscriberIdAsync(command.SubscriberId);
            var blacklistEntriesTask = _blacklistRepository.GetAllBySubscriberAsync(command.SubscriberId);
            var complaintsTask = _notificationComplaintRepository.GetComplaintsBySubscriberIdAsync(command.SubscriberId);

            await Task.WhenAll(
                apiKeysTask,
                channelConfigurationsTask,
                notificationLogsTask,
                templatesTask,
                workflowsTask,
                blacklistEntriesTask,
                complaintsTask
            );

            var exportData = new ExportDataDto
            {
                DataOwnerSubscriberId = command.SubscriberId,
                ExportedAtUtc = DateTime.UtcNow,
                Subscriber = new SubscriberDto
                {
                    CompanyName = subscriber.CompanyName.Name,
                    Email = subscriber.Email.Value,
                    PhoneNumber = subscriber.PhoneNumber?.Value,
                    ActiveChannels = subscriber.ActiveChannels.Select(c => c.Name).ToList(),
                    ExtraCredits = subscriber.ExtraCredits,
                    Subscription = new SubscriptionDto
                    {
                        Plan = subscription.Plan.ToString(),
                        MonthlyLimit = subscription.MonthlyLimit,
                        UsedThisMonth = subscription.UsedThisMonth,
                        ExpiresAt = subscription.ExpiresAt
                    }
                },
                ApiKeys = apiKeysTask.Result
                    .Select(key => new ApiKeyDto
                    {
                        Id = key.Id,
                        Prefix = key.Prefix,
                        Name = key.Name,
                        IsActive = key.IsActive,
                        CreatedAt = key.CreatedAt
                    })
                    .ToList(),
                ChannelConfigurations = channelConfigurationsTask.Result
                    .Select(setting => new ChannelConfigurationDto
                    {
                        Channel = setting.Channel.Name,
                        Settings = new Dictionary<string, string>(setting.Settings)
                    })
                    .ToList(),
                NotificationLogs = notificationLogsTask.Result
                    .Select(log => new NotificationLogDto
                    {
                        Id = log.Id,
                        Recipient = log.Recipient,
                        Channel = log.Channel.Name,
                        EventType = log.EventType.Name,
                        Subject = log.Content.Subject,
                        Body = log.Content.Body,
                        Status = log.Status.ToString(),
                        ErrorMessage = log.ErrorMessage,
                        CreatedAt = log.CreatedAt
                    })
                    .ToList(),
                NotificationTemplates = templatesTask.Result
                    .Select(template => new NotificationTemplateDto
                    {
                        Id = template.Id,
                        Name = template.Name,
                        Description = template.Description,
                        EventType = template.EventType.Name,
                        Channel = template.Channel.Name,
                        Subject = template.Content.Subject,
                        Body = template.Content.Body,
                        Source = template.SubscriberId == null ? "Default" : "Custom"
                    })
                    .ToList(),
                WorkflowNotifications = workflowsTask.Result
                    .Select(workflow => new WorkflowNotificationDto
                    {
                        Id = workflow.Id,
                        Name = workflow.Name,
                        Description = workflow.Description,
                        Channel = workflow.Channel.Name,
                        EventTrigger = workflow.EventTrigger,
                        CronExpression = workflow.CronExpression,
                        Recipients = workflow.Recipients,
                        IsActive = workflow.IsActive,
                        CreatedAt = workflow.CreatedAt,
                        Subject = workflow.Content.Subject,
                        Body = workflow.Content.Body
                    })
                    .ToList(),
                BlacklistEntries = blacklistEntriesTask.Result
                    .Select(entry => new BlacklistEntryDto
                    {
                        Id = entry.Id,
                        RecipientAddress = entry.RecipientAddress,
                        Channels = entry.UnwantedChannels.Select(channel => channel.Name).ToList(),
                        IsActive = entry.IsActive,
                        CreatedAt = entry.CreatedAt
                    })
                    .ToList(),
                NotificationComplaints = complaintsTask.Result
                    .Select(complaint => new NotificationComplaintDto
                    {
                        Id = complaint.Id,
                        ReporterEmail = complaint.ReporterEmail.Value,
                        Reason = complaint.Reason.ToString(),
                        Description = complaint.Description,
                        Subject = complaint.Content.Subject,
                        Body = complaint.Content.Body,
                        CreatedAt = complaint.CreatedAt
                    })
                    .ToList()
            };

            return exportData;
        }
    }
}
