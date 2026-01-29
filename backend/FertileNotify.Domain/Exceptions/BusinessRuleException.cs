namespace FertileNotify.Domain.Exceptions
{
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message, string errorCode = "BUSINESS_ERROR", object? details = null)
            : base(message, errorCode, 400, details) { }
    }
}
