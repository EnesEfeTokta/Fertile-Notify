namespace FertileNotify.API.Models
{
    public class GetCustomTemplatesRequest
    {
        public GetCustomTemplateQuery[] Queries { get; set; } = default!;
    }

    public class GetCustomTemplateQuery
    {
        public string EventType { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
