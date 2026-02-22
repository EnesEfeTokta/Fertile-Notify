namespace FertileNotify.Application.DTOs
{
    public class StatisticsDto
    {
        public int TotalUsage { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public Dictionary<string, int> StatsByChannel { get; set; } = new();
        public Dictionary<string, int> StatsByEventType { get; set; } = new();
    }
}
