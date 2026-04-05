namespace FertileNotify.Application.DTOs.Notifications;

public class ChannelConfigurationDto
{
	public string Channel { get; set; } = string.Empty;
	public Dictionary<string, string> Settings { get; set; } = new();
}
