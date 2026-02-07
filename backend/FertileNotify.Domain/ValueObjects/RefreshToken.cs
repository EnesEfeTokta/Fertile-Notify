namespace FertileNotify.Domain.ValueObjects
{
    public sealed class RefreshToken
    {
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }

        private RefreshToken() 
        {
            Token = string.Empty;
            ExpiresAt = DateTime.UtcNow;
            IsRevoked = false;
        }

        public RefreshToken(string token, DateTime expires)
        {
            Token = token;
            ExpiresAt = expires;
            IsRevoked = false;
        }

        public bool IsActive() => !IsRevoked && DateTime.UtcNow < ExpiresAt;

        public void Revoke() => IsRevoked = true;
    }
}
