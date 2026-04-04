namespace FertileNotify.Application.DTOs.Subscribers
{
    public class SubscriberDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<string> ActiveChannels { get; set; } = new();
        public int ExtraCredits { get; set; }
        public SubscriptionDto? Subscription { get; set; }
    }

    public class SubscriptionDto
    {
        public string Plan { get; set; } = string.Empty;
        public int MonthlyLimit { get; set; }
        public int UsedThisMonth { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
