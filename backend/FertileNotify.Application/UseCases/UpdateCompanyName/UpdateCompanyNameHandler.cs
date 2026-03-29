namespace FertileNotify.Application.UseCases.UpdateCompanyName
{
    public class UpdateCompanyNameHandler : IRequestHandler<UpdateCompanyNameCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdateCompanyNameHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<Unit> Handle(UpdateCompanyNameCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");
                
            var companyName = CompanyName.Create(command.CompanyName);

            subscriber.WithCompanyName(companyName);
            await _subscriberRepository.SaveAsync(subscriber);
            return Unit.Value;
        }
    }
}
