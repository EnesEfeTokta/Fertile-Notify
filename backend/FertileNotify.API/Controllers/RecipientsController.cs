using FertileNotify.API.Models.Requests;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.NotificationComplaint;
using FertileNotify.Application.UseCases.Unsubscribe;
using MediatR;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using FertileNotify.Domain.Entities;
using FertileNotify.API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/recipients")]
    public class RecipientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly INotificationComplaintRepository _complaintRepository;

        public RecipientsController(
            IMediator mediator,
            IBlacklistRepository blacklistRepository,
            INotificationComplaintRepository complaintRepository)
        {
            _mediator = mediator;
            _blacklistRepository = blacklistRepository;
            _complaintRepository = complaintRepository;
        }

        #region Public Processes
        [AllowAnonymous]
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
        {
            var command = new UnsubscribeCommand
            {
                SubscriberId = request.SubscriberId,
                Recipient = request.Recipient,
                Token = request.Token,
                Channels = request.Channels
            };

            await _mediator.Send(command);
            return Ok(ApiResponse<object>.SuccessResult(null!, "You have been successfully unsubscribed."));
        }

        [AllowAnonymous]
        [HttpPost("complaint")]
        public async Task<IActionResult> SubmitComplaint([FromBody] ComplaintRequest request)
        {
            await _mediator.Send(new NotificationComplaintCommand
            {
                SubscriberId = request.SubscriberId,
                ReporterEmail = request.ReporterEmail,
                Reason = request.Reason,
                Description = request.Description,
                Subject = request.NotificationSubject,
                Body = request.NotificationBody
            });
            return Ok(ApiResponse<object>.SuccessResult(null!, "Complaint submitted successfully."));
        }
        #endregion

        #region Private Processes
        [Authorize]
        [HttpGet("complaints")]
        public async Task<IActionResult> GetComplaints()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var complaints = await _complaintRepository.GetComplaintsBySubscriberIdAsync(subscriberId);
            return Ok(ApiResponse<object>.SuccessResult(complaints, "Complaints retrieved successfully."));
        }

        [Authorize]
        [HttpGet("blacklist")]
        public async Task<IActionResult> GetBlacklist()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var items = await _blacklistRepository.GetAllBySubscriberAsync(subscriberId);
            return Ok(ApiResponse<object>.SuccessResult(items, "Blacklist retrieved successfully."));
        }

        [Authorize]
        [HttpPost("blacklist")]
        public async Task<IActionResult> AddToBlacklist([FromBody] BanRecipientRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var entry = new ForbiddenRecipient(
                subscriberId,
                request.RecipientAddress,
                request.Channels.Select(NotificationChannel.From).ToList()
            );
            await _blacklistRepository.AddOrUpdateAsync(entry);
            return Ok(ApiResponse<object>.SuccessResult(null!, "Recipient added to blacklist."));
        }

        [Authorize]
        [HttpDelete("blacklist/{id}")]
        public async Task<IActionResult> RemoveFromBlacklist(Guid id)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var entry = await _blacklistRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Entry not found.");

            if (entry.UnwantedSubscriber != subscriberId)
                throw new UnauthorizedException("Access denied.");

            await _blacklistRepository.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null!, "Removed from blacklist."));
        }
        #endregion

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("User ID claim not found.");
            return Guid.Parse(claim.Value);
        }
    }
}