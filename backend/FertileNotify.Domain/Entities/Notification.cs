namespace FertileNotify.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationChannel Channel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}