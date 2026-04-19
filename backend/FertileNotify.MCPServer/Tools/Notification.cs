using FertileNotify.Application.UseCases.Workflow;
using MassTransit.Mediator;
using ModelContextProtocol.Server;

namespace FertileNotify.MCPServer.Tools
{
    public class Notification
    {
        private readonly IMediator _mediator;

        public Notification(IMediator mediator)
        {
            _mediator = mediator;
        }

        [McpServerTool("get_notification_status", "Get the status of a notification")]
        public async Task<string> GetStatus(Guid notificationId)
        {
            var result = await _mediator.Send(new GetWorkflowNotificationQuery(notificationId));
            return $"Durum: {result.Status}, Kanal: {result.Channel}";
        }
    }
}
