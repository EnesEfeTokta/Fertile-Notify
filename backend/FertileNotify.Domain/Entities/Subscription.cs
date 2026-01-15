namespace FertileNotify.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Channel { get; set; }
        public DateTime SubscribedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}