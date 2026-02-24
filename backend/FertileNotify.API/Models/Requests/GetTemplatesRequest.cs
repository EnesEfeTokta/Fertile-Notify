namespace FertileNotify.API.Models.Requests
{
    public class GetTemplatesRequest
    {
        public bool IsTemplateTypeCustom { get; set; } = false;
        public TemplateQuery[] Queries { get; set; } = default!;
    }

    public class TemplateQuery
    {
        public string EventType { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }
}
