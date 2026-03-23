using System;

namespace FertileNotify.Application.UseCases.DeleteAccount
{
    public class DeleteAccountCommand
    {
        public Guid SubscriberId { get; set; }
    }
}
