using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/templates")]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly INotificationLogRepository _logRepository;

        public TemplatesController(ITemplateRepository templateRepository, INotificationLogRepository logRepository)
        {

            _templateRepository = templateRepository;
            _logRepository = logRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var templates = await _templateRepository.GetAllTemplatesAsync(subscriberId);

            var result = templates.Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.EventType,
                t.Channel,
                t.Subject,
                t.Body,
                Source = t.SubscriberId == null ? "Default" : "Custom"
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Templates retrieved successfully."));
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var logs = await _logRepository.GetLatestBySubscriberIdAsync(subscriberId, 10);

            var result = logs.Select(t => new
            {
                t.Id,
                t.Recipient,
                t.Channel,
                t.EventType,
                t.Subject,
                t.Body,
                t.CreatedAt
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Notification logs retrieved successfully."));
        }

        [HttpPost("query")]
        public async Task<IActionResult> GetTemplates([FromBody] GetTemplatesRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var templates = new List<NotificationTemplate>();

            foreach (var query in request.Queries)
            {
                var eventType = EventType.From(query.EventType);
                var channel = NotificationChannel.From(query.Channel);

                var template = request.IsTemplateTypeCustom ? 
                    await _templateRepository.GetCustomTemplateAsync(eventType, channel, subscriberId) : await _templateRepository.GetGlobalTemplateAsync(eventType, channel); ;

                if (template != null) 
                    templates.Add(template);
            }

            var result = templates.Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.EventType,
                t.Channel,
                t.Subject,
                t.Body,
                Source = t.SubscriberId == null ? "Default" : "Custom"
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Queried templates retrieved successfully."));
        }

        [HttpPost("create-or-update-custom")]
        public async Task<IActionResult> CreateOrUpdateCustom([FromBody] CreateTemplateRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var eventType = EventType.From(request.EventType);
            var channel = NotificationChannel.From(request.Channel);

            var existingTemplate = await _templateRepository.GetCustomTemplateAsync(eventType, channel,subscriberId);

            if (existingTemplate != null)
            {
                existingTemplate.Update(request.Name, request.Description, request.Subject, request.Body);
                await _templateRepository.SaveAsync();
            }
            else
            {
                var newTemplate = NotificationTemplate.CreateCustom(
                    subscriberId, 
                    request.Name, 
                    request.Description, 
                    eventType, 
                    channel, 
                    request.Subject, 
                    request.Body
                );
                await _templateRepository.AddAsync(newTemplate);
            }

            return Ok(ApiResponse<object>.SuccessResult(default!, "Template created or updated successfully."));
        }

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(subscriberIdClaim.Value);
        }
    }
}
