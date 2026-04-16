namespace FertileNotify.API.Models.Requests
{
    public class UpdateCompanyInfoRequest
    {
        public string CompanyName { get; init; } = string.Empty;
        public string CompanyDescription { get; init; } = string.Empty;
        public string LogoUrl { get; init; } = string.Empty;
        public string WebsiteUrl { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
    }
}
