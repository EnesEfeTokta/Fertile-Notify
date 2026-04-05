namespace FertileNotify.Application.UseCases.DeleteAccount
{
    public class DeleteAccountCommand: IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
    }
}
