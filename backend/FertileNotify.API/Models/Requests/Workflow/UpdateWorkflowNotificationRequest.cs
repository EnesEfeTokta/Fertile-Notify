using System;

namespace FertileNotify.API.Models.Requests;

public class UpdateWorkflowNotificationRequest
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? EventType { get; set; }
	public string? Channel { get; set; }
	public string? EventTrigger { get; set; }
	public string? CronExpression { get; set; }
	public string? Subject { get; set; }
	public string? Body { get; set; }
	public List<ChannelRecipientGroup>? To { get; set; }
}
