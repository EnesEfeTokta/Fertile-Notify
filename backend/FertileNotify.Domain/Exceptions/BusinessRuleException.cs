namespace FertileNotify.Domain.Exceptions
{
    public class BusinessRuleException : Exception
    {
        protected BusinessRuleException (string message) : base(message) { }
    }
}
