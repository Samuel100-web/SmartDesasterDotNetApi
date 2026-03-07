using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Enums;
using SmartResponse.Core.Interfaces;
using SmartResponse.Core.Utils;
using System.Security.Claims;

namespace SmartResponse.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public decimal? ReporterLiveLat { get; private set; }
        public decimal? ReporterLiveLong { get; private set; }

        public IncidentsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //Create Incident (Matches IncidentCreateDto)
        [HttpPost]
        public async Task<IActionResult> CreateIncident(IncidentCreateDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            var userId = Guid.Parse(userIdString);

            // Geofencing Logic
            bool isVerifiedLocation = false;
            if (dto.ReporterLiveLat.HasValue && dto.ReporterLiveLong.HasValue)
            {
                var distance = LocationHelper.CalculateDistance(
                    dto.TargetLat, dto.TargetLong,
                    dto.ReporterLiveLat.Value, dto.ReporterLiveLong.Value);

                if (distance <= 5.0) isVerifiedLocation = true;
            }

            var incident = new Incident
            {
                Title = dto.Title,
                Description = dto.Description,
                IncidentTypeId = dto.IncidentTypeId,
                Severity = dto.Severity,
                TargetLat = dto.TargetLat,
                TargetLong = dto.TargetLong,
                ReporterId = userId,                
                ReporterLiveLat = dto.ReporterLiveLat,
                ReporterLiveLong = dto.ReporterLiveLong,
                DataSource = "Manual",
                Status = IncidentStatus.Pending
            };

            await _uow.Incidents.AddAsync(incident);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Success", IncidentId = incident.Id, LocationVerified = isVerifiedLocation });
        }
        
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<IncidentSummaryDto>>> GetLiveIncidents()
        {
            var incidents = await _uow.Incidents.FindAsync(
                i => i.Status == IncidentStatus.Verified,
                i => i.IncidentType
            );

            var result = incidents.Select(i => new IncidentSummaryDto(
                i.Id,
                i.Title,
                i.IncidentType.Name,
                i.Severity,
                i.Status,
                i.TargetLat,
                i.TargetLong,
                i.CreatedAt
            ));

            return Ok(result);
        }

        //Incident Detail (Matches your IncidentDetailDto)
        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentDetailDto>> GetIncidentById(Guid id)
        {
            var incidentList = await _uow.Incidents.FindAsync(
                i => i.Id == id,
                i => i.IncidentType, i => i.Media, i => i.Reporter!, i => i.Votes, i => i.Verifications
            );

            var incident = incidentList.FirstOrDefault();
            if (incident == null) return NotFound();

            // Mapping exactly to your class IncidentDetailDto
            var dto = new IncidentDetailDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                Severity = incident.Severity,
                Status = incident.Status,
                CreatedAt = incident.CreatedAt,
                DataSource = incident.DataSource,
                Lat = incident.TargetLat,
                Long = incident.TargetLong,
                ReporterName = incident.Reporter?.FullName ?? "Anonymous",

                // TotalVotes calculate kar rahe hain (Sum of Upvotes/Downvotes)
                TotalVotes = incident.Votes.Sum(v => v.VoteValue),

                Type = new LookupDto(incident.IncidentType.Id, incident.IncidentType.Name, incident.IncidentType.Icon),

                Media = incident.Media.Select(m => new IncidentMediaDto(m.Id, m.Url, m.MediaType)).ToList(),

                Verifications = incident.Verifications.Select(v => new VerificationDto(
                    v.Id,
                    v.Volunteer?.FullName ?? "Volunteer",
                    v.Action,
                    v.Remarks,
                    v.CreatedAt)).ToList()
            };

            return Ok(dto);
        }

        //PUT Method: Full Incident Update
        [Authorize(Roles = "Admin,Volunteer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncident(Guid id, IncidentUpdateDto dto)
        {            
            var incident = await _uow.Incidents.GetByIdAsync(id);
            if (incident == null) return NotFound("Incident not found");
            
            incident.Title = dto.Title;
            incident.Description = dto.Description;
            incident.IncidentTypeId = dto.IncidentTypeId;
            incident.Severity = dto.Severity;
            incident.TargetLat = dto.TargetLat;
            incident.TargetLong = dto.TargetLong;
            ReporterLiveLat = dto.ReporterLiveLat;
            ReporterLiveLong = dto.ReporterLiveLong;
            incident.UpdatedAt = DateTime.UtcNow;
            
            _uow.Incidents.Update(incident);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Incident updated successfully" });
        }

        //Update Status (Matches IncidentStatusPatchDto)
        [Authorize(Roles = "Admin,Volunteer")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, IncidentStatusPatchDto dto)
        {
            var incident = await _uow.Incidents.GetByIdAsync(id);
            if (incident == null) return NotFound();

            var volunteerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Update Incident Status
            incident.Status = dto.NewStatus;

            // Create Audit Trail (Verification)
            var verification = new IncidentVerification
            {
                IncidentId = id,
                VolunteerId = volunteerId,
                Action = dto.NewStatus.ToString(),
                Remarks = dto.Remarks
            };

            await _uow.IncidentVerifications.AddAsync(verification);
            _uow.Incidents.Update(incident);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Status updated" });
        }

        //Soft Delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(Guid id)
        {
            var incident = await _uow.Incidents.GetByIdAsync(id);
            if (incident == null) return NotFound();

            _uow.Incidents.Delete(incident);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Deleted" });
        }
    }
}