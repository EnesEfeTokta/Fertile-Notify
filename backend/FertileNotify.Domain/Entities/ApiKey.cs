namespace FertileNotify.Domain.Entities
{
    public class ApiKey
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string KeyHash { get; private set; } = string.Empty;
        public string Prefix { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Scopes { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private ApiKey() { }

        public ApiKey(Guid subscriberId, string keyHash, string prefix, string name, string? scopes = null)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            KeyHash = keyHash;
            Prefix = prefix;
            Name = name;
            Scopes = scopes;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Revoke() => IsActive = false;

        public void SetActive(bool isActive)
            => IsActive = isActive;

        public void SetScopes(string? scopes)
            => Scopes = string.IsNullOrWhiteSpace(scopes) ? null : scopes.Trim();

    }
}
