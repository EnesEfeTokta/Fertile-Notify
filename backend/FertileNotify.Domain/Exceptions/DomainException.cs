namespace FertileNotify.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }
        public object? Details { get; }

        protected DomainException(string message, string errorCode, int statusCode, object? details = null) 
            : base(message) 
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Details = details;
        }
    }
}
