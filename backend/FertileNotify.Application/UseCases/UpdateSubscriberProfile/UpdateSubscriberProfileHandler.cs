using FertileNotify.Domain.ValueObjects;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Application.Interfaces.Subscribers;
using MediatR;

namespace FertileNotify.Application.UseCases.UpdateSubscriberProfile
{
    public class UpdateSubscriberProfileHandler : IRequestHandler<UpdateSubscriberProfileCommand, Unit>
    {
        private readonly ISubscriberRepository _repo;

        public UpdateSubscriberProfileHandler(ISubscriberRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(UpdateSubscriberProfileCommand cmd, CancellationToken ct)
        {
            var subscriber = await _repo.GetByIdAsync(cmd.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            // ── Company fields ──
            if (cmd.CompanyName is not null)
                subscriber.WithCompanyName(CompanyName.Create(cmd.CompanyName));

            if (cmd.CompanyDescription is not null)
                subscriber.WithCompanyDescription(cmd.CompanyDescription);

            if (cmd.LogoUrl is not null)
                subscriber.WithLogoUrl(string.IsNullOrWhiteSpace(cmd.LogoUrl)
                    ? null
                    : CustomUrl.Create(cmd.LogoUrl));

            if (cmd.WebsiteUrl is not null)
                subscriber.WithWebsiteUrl(string.IsNullOrWhiteSpace(cmd.WebsiteUrl)
                    ? null
                    : CustomUrl.Create(cmd.WebsiteUrl));

            if (cmd.Location is not null)
                subscriber.WithLocation(cmd.Location);

            // ── Contact fields ──
            if (cmd.Email is not null)
                subscriber.WithEmail(EmailAddress.Create(cmd.Email));

            if (cmd.PhoneNumber is not null)
                subscriber.WithPhoneNumber(string.IsNullOrWhiteSpace(cmd.PhoneNumber)
                    ? null
                    : PhoneNumber.Create(cmd.PhoneNumber.Trim()));

            await _repo.SaveAsync(subscriber);
            return Unit.Value;
        }
    }
}
