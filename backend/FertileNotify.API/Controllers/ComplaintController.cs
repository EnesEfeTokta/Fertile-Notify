using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertileNotify.API.Models.Requests;
using FertileNotify.Application.UseCases.NotificationComplaint;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Interfaces;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    [Authorize]
    public class ComplaintController : ControllerBase
    {
        private readonly NotificationComplaintHandler _complaintHandler;
        private readonly INotificationComplaintRepository _complaintRepository;

        public ComplaintController(NotificationComplaintHandler complaintHandler, INotificationComplaintRepository complaintRepository)
        {
            _complaintHandler = complaintHandler;
            _complaintRepository = complaintRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ComplaintSubmit([FromBody] ComplaintRequest request)
        {
            await _complaintHandler.HandleAsync(new NotificationComplaintCommand
            {
                SubscriberId = request.SubscriberId,
                ReporterEmail = request.ReporterEmail,
                Reason = request.Reason,
                Description = request.Description,
                NotificationSubject = request.NotificationSubject,
                NotificationBody = request.NotificationBody
            });

            return Ok(ApiResponse<object>.SuccessResult(null!, "Complaint submitted successfully."));
        }

        [HttpGet("{subscriberId}")]
        public IActionResult GetComplaints(Guid subscriberId)
        {
            var complaints = _complaintRepository.GetComplaintsBySubscriberIdAsync(subscriberId);
            var result = complaints.Result.Select(c => new
            {
                c.Id,
                c.ReporterEmail,
                Reason = c.Reason.ToString(),
                c.Description,
                c.NotificationSubject,
                c.NotificationBody,
                c.CreatedAt
            });
            return Ok(ApiResponse<object>.SuccessResult(result, "Complaints retrieved successfully."));
        }
    }
}
