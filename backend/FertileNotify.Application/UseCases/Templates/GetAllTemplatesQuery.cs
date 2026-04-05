namespace FertileNotify.Application.UseCases.Templates
{
    public class GetAllTemplatesQuery : IQuery<List<TemplateResponseItem>>
    {
        public Guid SubscriberId { get; set; }
    }

    public class GetAllTemplatesHandler : IQueryHandler<GetAllTemplatesQuery, List<TemplateResponseItem>>
    {
        private readonly ITemplateRepository _templateRepository;

        public GetAllTemplatesHandler(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<List<TemplateResponseItem>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _templateRepository.GetAllTemplatesAsync(request.SubscriberId);
            return templates.Select(TemplateResponseItem.FromTemplate).ToList();
        }
    }
}