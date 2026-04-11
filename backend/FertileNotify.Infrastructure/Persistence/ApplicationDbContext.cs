using System.Reflection;

namespace FertileNotify.Infrastructure.Persistence
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<SubscriberChannelSetting> SubscriberChannelSettings { get; set; }
        public DbSet<SubscriberDailyStats> DailyStats { get; set; }
        public DbSet<ForbiddenRecipient> ForbiddenRecipients { get; set; }
        public DbSet<NotificationComplaint> NotificationComplaints { get; set; }
        public DbSet<AutomationWorkflow> AutomationWorkflows { get; set; }
        public DbSet<SystemNotification> SystemNotifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureIgnoredDomainTypes(modelBuilder);

            foreach (var assembly in GetConfigurationAssemblies())
            {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }

            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        protected virtual void ConfigureIgnoredDomainTypes(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<EventType>();
            modelBuilder.Ignore<NotificationChannel>();
        }

        protected virtual IEnumerable<Assembly> GetConfigurationAssemblies()
        {
            yield return Assembly.GetExecutingAssembly();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
