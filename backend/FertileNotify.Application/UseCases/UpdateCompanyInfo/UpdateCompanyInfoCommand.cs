namespace FertileNotify.Application.UseCases.UpdateCompanyInfo
{
    public class UpdateCompanyInfoCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; init; }
        public string CompanyName { get; init; } = string.Empty;
        public string CompanyDescription { get; init; } = string.Empty;
        public string LogoUrl { get; init; } = string.Empty;
        public string WebsiteUrl { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
    }
}
