namespace FertileNotify.API.Models
{
    public class CreateTemplateRequest
    {
        public string EventType { get; set; } = null!;
        public string SubjectTemplate { get; set; } = null!;
        public string BodyTemplate { get; set; } = null!;
    }
}
