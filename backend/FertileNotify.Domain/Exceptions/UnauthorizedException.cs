namespace FertileNotify.Domain.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message, string errorCode = "UNAUTHORIZED")
            : base(message, errorCode, 401) { }
    }
}
