namespace FertileNotify.Application.UseCases.Templates
{
    public class GetTemplatesQuery : IQuery<List<TemplateResponseItem>>
    {
        public Guid SubscriberId { get; set; }
        public bool IsTemplateTypeCustom { get; set; }
        public List<TemplateQueryItem> Queries { get; set; } = new();
    }

    public class GetTemplatesHandler : IQueryHandler<GetTemplatesQuery, List<TemplateResponseItem>>
    {
        private readonly ITemplateRepository _templateRepository;

        public GetTemplatesHandler(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<List<TemplateResponseItem>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = new List<NotificationTemplate>();

            foreach (var query in request.Queries)
            {
                var eventType = EventType.From(query.EventType);
                var channel = NotificationChannel.From(query.Channel);

                var template = request.IsTemplateTypeCustom
                    ? await _templateRepository.GetCustomTemplateAsync(eventType, channel, request.SubscriberId)
                    : await _templateRepository.GetGlobalTemplateAsync(eventType, channel);

                if (template != null)
                    templates.Add(template);
            }

            return templates.Select(TemplateResponseItem.FromTemplate).ToList();
        }
    }
}