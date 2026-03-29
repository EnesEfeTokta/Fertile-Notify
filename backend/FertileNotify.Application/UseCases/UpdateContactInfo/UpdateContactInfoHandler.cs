namespace FertileNotify.Application.UseCases.UpdateContactInfo
{
    public class UpdateContactInfoHandler : IRequestHandler<UpdateContactInfoCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdateContactInfoHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<Unit> Handle(UpdateContactInfoCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            var email = EmailAddress.Create(command.Email);
            var phoneNumber = PhoneNumber.Create(command.PhoneNumber?.Trim() ?? string.Empty);

            subscriber.WithEmail(email).WithPhoneNumber(phoneNumber);
            await _subscriberRepository.SaveAsync(subscriber);
            return Unit.Value;
        }
    }
}
