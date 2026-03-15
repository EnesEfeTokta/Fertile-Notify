using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.UpdatePassword
{
    public class UpdatePasswordHandler
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdatePasswordHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task HandleAsync(UpdatePasswordCommand command)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            if (!subscriber.Password.Verify(command.CurrentPassword))
                throw new UnauthorizedException("Current password is incorrect.");

            subscriber.UpdatePassword(Password.Create(command.NewPassword));

            await _subscriberRepository.SaveAsync(subscriber);
        }
    }
}
