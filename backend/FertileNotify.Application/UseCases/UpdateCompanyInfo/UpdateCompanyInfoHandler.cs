using FertileNotify.Domain.ValueObjects;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.Interfaces.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.UseCases.UpdateCompanyInfo
{
    public class UpdateCompanyInfoHandler : IRequestHandler<UpdateCompanyInfoCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public UpdateCompanyInfoHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<Unit> Handle(UpdateCompanyInfoCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId, cancellationToken)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.WithCompanyName(CompanyName.Create(command.CompanyName))
                      .WithCompanyDescription(command.CompanyDescription)
                      .WithLogoUrl(string.IsNullOrWhiteSpace(command.LogoUrl) ? null : CustomUrl.Create(command.LogoUrl))
                      .WithWebsiteUrl(string.IsNullOrWhiteSpace(command.WebsiteUrl) ? null : CustomUrl.Create(command.WebsiteUrl))
                      .WithLocation(command.Location);
            
            await _subscriberRepository.SaveAsync(subscriber, cancellationToken);
            return Unit.Value;
        }
    }
}
