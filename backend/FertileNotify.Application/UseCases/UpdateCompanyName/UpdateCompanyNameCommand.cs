namespace FertileNotify.Application.UseCases.UpdateCompanyName
{
    public class UpdateCompanyNameCommand
    {
        public Guid SubscriberId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}
