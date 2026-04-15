namespace FertileNotify.Application.UseCases.UpdateCompanyDescription
{
    public class UpdateCompanyDescriptionHandler : IRequestHandler<UpdateCompanyDescriptionCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdateCompanyDescriptionHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<Unit> Handle(UpdateCompanyDescriptionCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");;

            subscriber.WithCompanyDescription(command.CompanyDescription);
            await _subscriberRepository.SaveAsync(subscriber);
            return Unit.Value;
        }
    }
}
