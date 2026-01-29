namespace FertileNotify.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message, string errorCode = "NOT_FOUND")
            : base(message, errorCode, 404) { }
    }
}
