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
                
            var companyName = CompanyName.Create(command.CompanyName);

            subscriber.WithCompanyName(companyName);
            await _subscriberRepository.SaveAsync(subscriber);
        }
    }
}
