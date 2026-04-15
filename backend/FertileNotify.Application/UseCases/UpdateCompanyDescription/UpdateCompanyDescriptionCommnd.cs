namespace FertileNotify.Application.UseCases.UpdateCompanyDescription
{
    public class UpdateCompanyDescriptionCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public string CompanyDescription { get; set; } = string.Empty;
    }
}
