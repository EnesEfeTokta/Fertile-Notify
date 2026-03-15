using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.UpdateContactInfo
{
    public class UpdateContactInfoHandler
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdateContactInfoHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task HandleAsync(UpdateContactInfoCommand command)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.UpdateContactInfo(
                EmailAddress.Create(command.Email),
                string.IsNullOrWhiteSpace(command.PhoneNumber)
                    ? null
                    : PhoneNumber.Create(command.PhoneNumber)
            );

            await _subscriberRepository.SaveAsync(subscriber);
        }
    }
}
