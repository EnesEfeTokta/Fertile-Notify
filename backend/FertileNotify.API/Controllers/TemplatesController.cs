using FertileNotify.API.Models;
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

        public TemplatesController(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var templates = await _templateRepository.GetAllTemplatesAsync(subscriberId);

            return Ok(templates.Select(t => new
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Event = t.EventType.Name,
                Channel = t.Channel.Name,
                Subject = t.SubjectTemplate,
                Body = t.BodyTemplate,
                Source = t.SubscriberId == null ? "Default" : "Custom"
            }));
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

            return Ok(templates.Select(t => new
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Event = t.EventType.Name,
                Channel = t.Channel.Name,
                Subject = t.SubjectTemplate,
                Body = t.BodyTemplate,
                Source = t.SubscriberId == null ? "Default" : "Custom"
            }));
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
                existingTemplate.Update(request.Name, request.Description, request.SubjectTemplate, request.BodyTemplate);
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
                    request.SubjectTemplate, 
                    request.BodyTemplate
                );
                await _templateRepository.AddAsync(newTemplate);
            }

            return Ok();
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
