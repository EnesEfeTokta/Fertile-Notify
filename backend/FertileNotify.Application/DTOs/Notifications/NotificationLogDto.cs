using System;

namespace FertileNotify.Application.DTOs.Notifications;

public class NotificationLogDto
{
	public Guid Id { get; set; }
	public string Recipient { get; set; } = string.Empty;
	public string Channel { get; set; } = string.Empty;
	public string EventType { get; set; } = string.Empty;
	public string Subject { get; set; } = string.Empty;
	public string Body { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public string? ErrorMessage { get; set; }
	public DateTime CreatedAt { get; set; }
}
