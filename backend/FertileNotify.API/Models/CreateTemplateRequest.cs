namespace FertileNotify.API.Models
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string Channel { get; set; } = null!;
        public string SubjectTemplate { get; set; } = null!;
        public string BodyTemplate { get; set; } = null!;
    }
}
