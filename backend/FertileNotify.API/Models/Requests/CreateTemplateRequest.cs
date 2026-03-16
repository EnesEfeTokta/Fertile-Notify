namespace FertileNotify.API.Models.Requests
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string Channel { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
