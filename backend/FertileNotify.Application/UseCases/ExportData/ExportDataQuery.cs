namespace FertileNotify.Application.UseCases.ExportData
{
    public class ExportDataQuery : IQuery<ExportDataDto>
    {
        public Guid SubscriberId { get; init; }
    }
}