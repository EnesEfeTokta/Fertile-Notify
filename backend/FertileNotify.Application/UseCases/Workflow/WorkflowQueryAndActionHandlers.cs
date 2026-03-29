namespace FertileNotify.Application.UseCases.Workflow
{
    public class GetWorkflowNotificationHandler : WorkflowHandlerBase, IRequestHandler<GetWorkflowNotificationQuery, WorkflowNotificationDto>
    {
        public GetWorkflowNotificationHandler(IAutomationRepository automationRepository) : base(automationRepository) { }

        public async Task<WorkflowNotificationDto> Handle(GetWorkflowNotificationQuery request, CancellationToken cancellationToken)
        {
            var workflow = await GetOwnedWorkflowAsync(request.SubscriberId, request.Id);
            return MapToDto(workflow);
        }
    }

    public class ListWorkflowNotificationsHandler : WorkflowHandlerBase, IRequestHandler<ListWorkflowNotificationsQuery, List<WorkflowNotificationDto>>
    {
        public ListWorkflowNotificationsHandler(IAutomationRepository automationRepository) : base(automationRepository) { }

        public async Task<List<WorkflowNotificationDto>> Handle(ListWorkflowNotificationsQuery request, CancellationToken cancellationToken)
        {
            var workflows = await _automationRepository.GetBySubscriberIdAsync(request.SubscriberId);
            return workflows.Select(MapToDto).ToList();
        }
    }

    public class DeleteWorkflowNotificationHandler : WorkflowHandlerBase, IRequestHandler<DeleteWorkflowNotificationCommand, Unit>
    {
        private readonly IWorkflowScheduleService _workflowScheduleService;

        public DeleteWorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            IWorkflowScheduleService workflowScheduleService) : base(automationRepository)
        {
            _workflowScheduleService = workflowScheduleService;
        }

        public async Task<Unit> Handle(DeleteWorkflowNotificationCommand request, CancellationToken cancellationToken)
        {
            var workflow = await GetOwnedWorkflowAsync(request.SubscriberId, request.Id);

            await _workflowScheduleService.RemoveAsync(workflow.Id);
            await _automationRepository.DeleteAsync(request.Id);
            await _automationRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }

    public class ActivateWorkflowNotificationHandler : WorkflowHandlerBase, IRequestHandler<ActivateWorkflowNotificationCommand, Unit>
    {
        private readonly IWorkflowScheduleService _workflowScheduleService;

        public ActivateWorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            IWorkflowScheduleService workflowScheduleService) : base(automationRepository)
        {
            _workflowScheduleService = workflowScheduleService;
        }

        public async Task<Unit> Handle(ActivateWorkflowNotificationCommand request, CancellationToken cancellationToken)
        {
            var workflow = await GetOwnedWorkflowAsync(request.SubscriberId, request.Id);
            workflow.Activate();
            _automationRepository.Update(workflow);
            await _automationRepository.SaveChangesAsync();
            await _workflowScheduleService.SyncAsync(workflow);

            return Unit.Value;
        }
    }

    public class DeactivateWorkflowNotificationHandler : WorkflowHandlerBase, IRequestHandler<DeactivateWorkflowNotificationCommand, Unit>
    {
        private readonly IWorkflowScheduleService _workflowScheduleService;

        public DeactivateWorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            IWorkflowScheduleService workflowScheduleService) : base(automationRepository)
        {
            _workflowScheduleService = workflowScheduleService;
        }

        public async Task<Unit> Handle(DeactivateWorkflowNotificationCommand request, CancellationToken cancellationToken)
        {
            var workflow = await GetOwnedWorkflowAsync(request.SubscriberId, request.Id);
            workflow.Deactivate();
            _automationRepository.Update(workflow);
            await _automationRepository.SaveChangesAsync();
            await _workflowScheduleService.RemoveAsync(workflow.Id);

            return Unit.Value;
        }
    }
}
