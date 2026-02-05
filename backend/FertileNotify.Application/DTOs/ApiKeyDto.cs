namespace FertileNotify.Application.DTOs
{
    public class ApiKeyDto
    {
        public Guid Id { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
