using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Enums;
using SmartResponse.Core.Interfaces;
using System.Security.Claims;

namespace SmartResponse.API.Controllers
{
    [Authorize(Roles = "Admin,Volunteer")]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentVerificationsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<IncidentVerificationsController> _logger;

        public IncidentVerificationsController(IUnitOfWork uow, ILogger<IncidentVerificationsController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        //POST: Incident ko Verify ya Reject karna
        [HttpPost]
        public async Task<IActionResult> VerifyIncident(VerificationCreateDto dto)
        {
            try
            {                
                var incident = await _uow.Incidents.GetByIdAsync(dto.IncidentId);
                if (incident == null) return NotFound("Incident not found.");

                var volunteerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                
                var verification = new IncidentVerification
                {
                    IncidentId = dto.IncidentId,
                    VolunteerId = volunteerId,
                    Action = dto.Action,
                    Remarks = dto.Remarks
                };
                
                if (dto.Action.Equals("Verified", StringComparison.OrdinalIgnoreCase))
                {
                    incident.Status = IncidentStatus.Verified;
                    
                    if (incident.ReporterId.HasValue)
                    {
                        var reporter = await _uow.Users.GetByIdAsync(incident.ReporterId.Value);
                        if (reporter != null) reporter.TrustScore += 10;
                    }
                }
                else if (dto.Action.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    incident.Status = IncidentStatus.Rejected;
                    
                    if (incident.ReporterId.HasValue)
                    {
                        var reporter = await _uow.Users.GetByIdAsync(incident.ReporterId.Value);
                        if (reporter != null) reporter.TrustScore -= 20;
                    }
                }
                
                await _uow.IncidentVerifications.AddAsync(verification);
                _uow.Incidents.Update(incident);

                await _uow.CompleteAsync();

                return Ok(new { Message = $"Incident has been marked as {dto.Action}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during incident verification");
                return StatusCode(500, "Internal server error occurred.");
            }
        }

        //GET Aik Incident ki poori Verification History (Audit Trail)
        [HttpGet("incident/{incidentId}")]
        public async Task<ActionResult<IEnumerable<VerificationDto>>> GetHistory(Guid incidentId)
        {
            try
            {                
                var verifications = await _uow.IncidentVerifications.FindAsync(
                    v => v.IncidentId == incidentId,
                    v => v.Volunteer
                );

                var result = verifications.Select(v => new VerificationDto(
                    v.Id,
                    v.Volunteer.FullName,
                    v.Action,
                    v.Remarks,
                    v.CreatedAt
                ));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching verification history for {incidentId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        //GET Specific Verification Detail By ID
        [HttpGet("{id}")]
        public async Task<ActionResult<VerificationDto>> GetById(Guid id)
        {
            try
            {
                var vList = await _uow.IncidentVerifications.FindAsync(x => x.Id == id, x => x.Volunteer);
                var v = vList.FirstOrDefault();

                if (v == null) return NotFound();

                return Ok(new VerificationDto(v.Id, v.Volunteer.FullName, v.Action, v.Remarks, v.CreatedAt));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching verification {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}