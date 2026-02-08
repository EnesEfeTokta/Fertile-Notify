using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FertileNotify.API.Models;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/templates")]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubscriberRepository _subscriberRepository;

        public TemplatesController(ITemplateRepository templateRepository, ISubscriberRepository subscriberRepository)
        {
            _templateRepository = templateRepository;
            _subscriberRepository = subscriberRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] CreateTemplateRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var eventType = EventType.From(request.EventType);

            var existingTemplate = await _templateRepository.GetTemplateAsync(eventType, subscriberId);

            if (existingTemplate != null)
            {
                existingTemplate.Update(request.SubjectTemplate, request.BodyTemplate);
            }
            else
            {
                var newTemplate = NotificationTemplate.CreateCustom(subscriberId, eventType, request.SubjectTemplate, request.BodyTemplate);
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
