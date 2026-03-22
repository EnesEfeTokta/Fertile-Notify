using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.ForgotPassword
{
    public class ForgotPasswordHandler
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(
            ISubscriberRepository subscriberRepository,
            IOtpService otpService,
            IEmailService emailService,
            ILogger<ForgotPasswordHandler> logger)
        {
            _subscriberRepository = subscriberRepository;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task HandleAsync(ForgotPasswordCommand command)
        {
            var emailAddress = EmailAddress.Create(command.Email);

            var subscriber = await _subscriberRepository.GetByEmailAsync(emailAddress)
                ?? throw new NotFoundException("No subscriber found with this email.");

            var otpCode = await _otpService.GenerateOtpAsync(subscriber.Id);

            await _emailService.SendEmailAsync(
                subscriber.Email, 
                "Password Reset OTP", 
                $"Your OTP for password reset is: {otpCode}. It is valid for 5 minutes."
            );

            _logger.LogInformation("Password reset OTP sent to {Email}", command.Email);
        }
    }
}