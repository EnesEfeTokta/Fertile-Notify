namespace FertileNotify.Application.UseCases.Workflow
{
    public class WorkflowNotificationHandler
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly AutomationTriggerService _automationTriggerService;
        private readonly IWorkflowScheduleService _workflowScheduleService;

        public WorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            AutomationTriggerService automationTriggerService,
            IWorkflowScheduleService workflowScheduleService)
        {
            _automationRepository = automationRepository;
            _automationTriggerService = automationTriggerService;
            _workflowScheduleService = workflowScheduleService;
        }

        public Task<int> TriggerAsync(TriggerWorkflowNotificationsCommand command)
            => new TriggerWorkflowNotificationsHandler(_automationTriggerService)
                .Handle(command, CancellationToken.None);

        public Task<Guid> CreateAsync(CreateWorkflowNotificationCommand command)
            => new CreateWorkflowNotificationHandler(_automationRepository, _workflowScheduleService)
                .Handle(command, CancellationToken.None);

        public Task UpdateAsync(UpdateWorkflowNotificationCommand command)
            => new UpdateWorkflowNotificationHandler(_automationRepository, _workflowScheduleService)
                .Handle(command, CancellationToken.None);

        public Task<List<WorkflowNotificationDto>> ListAsync(Guid subscriberId)
            => new ListWorkflowNotificationsHandler(_automationRepository)
                .Handle(new ListWorkflowNotificationsQuery { SubscriberId = subscriberId }, CancellationToken.None);

        public Task<WorkflowNotificationDto> GetAsync(WorkflowNotificationByIdCommand command)
            => new GetWorkflowNotificationHandler(_automationRepository)
                .Handle(new GetWorkflowNotificationQuery
                {
                    SubscriberId = command.SubscriberId,
                    Id = command.Id
                }, CancellationToken.None);

        public Task DeleteAsync(WorkflowNotificationByIdCommand command)
            => new DeleteWorkflowNotificationHandler(_automationRepository, _workflowScheduleService)
                .Handle(new DeleteWorkflowNotificationCommand
                {
                    SubscriberId = command.SubscriberId,
                    Id = command.Id
                }, CancellationToken.None);

        public Task ActivateAsync(WorkflowNotificationByIdCommand command)
            => new ActivateWorkflowNotificationHandler(_automationRepository, _workflowScheduleService)
                .Handle(new ActivateWorkflowNotificationCommand
                {
                    SubscriberId = command.SubscriberId,
                    Id = command.Id
                }, CancellationToken.None);

        public Task DeactivateAsync(WorkflowNotificationByIdCommand command)
            => new DeactivateWorkflowNotificationHandler(_automationRepository, _workflowScheduleService)
                .Handle(new DeactivateWorkflowNotificationCommand
                {
                    SubscriberId = command.SubscriberId,
                    Id = command.Id
                }, CancellationToken.None);
    }
}