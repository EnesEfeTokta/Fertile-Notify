namespace FertileNotify.Application.UseCases.UpdateCompanyName
{
    public class UpdateCompanyNameCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}
