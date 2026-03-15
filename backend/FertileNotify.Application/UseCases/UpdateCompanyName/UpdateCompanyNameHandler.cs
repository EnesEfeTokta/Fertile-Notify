using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.UpdateCompanyName
{
    public class UpdateCompanyNameHandler
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdateCompanyNameHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task HandleAsync(UpdateCompanyNameCommand command)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.UpdateCompanyName(CompanyName.Create(command.CompanyName));

            await _subscriberRepository.SaveAsync(subscriber);
        }
    }
}
