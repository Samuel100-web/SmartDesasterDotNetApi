using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;
using System.Security.Claims;

namespace SmartResponse.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentVotesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<IncidentVotesController> _logger;

        public IncidentVotesController(IUnitOfWork uow, ILogger<IncidentVotesController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        //POST Vote Submit or Update (Upsert Logic)
        [HttpPost]
        public async Task<IActionResult> CastVote(VoteCreateDto dto)
        {
            try
            {                
                if (dto.VoteValue != 1 && dto.VoteValue != -1)
                    return BadRequest("Vote value must be 1 (Upvote) or -1 (Downvote).");

                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                
                var existingVotes = await _uow.IncidentVotes.FindAsync(v =>
                    v.IncidentId == dto.IncidentId && v.UserId == userId);

                var existingVote = existingVotes.FirstOrDefault();

                if (existingVote != null)
                {                    
                    if (existingVote.VoteValue == dto.VoteValue)
                    {
                        _uow.IncidentVotes.Delete(existingVote);
                        await _uow.CompleteAsync();
                        return Ok(new { Message = "Vote removed" });
                    }
                    
                    existingVote.VoteValue = dto.VoteValue;
                    existingVote.UpdatedAt = DateTime.UtcNow;
                    _uow.IncidentVotes.Update(existingVote);
                }
                else
                {                    
                    var newVote = new IncidentVote
                    {
                        IncidentId = dto.IncidentId,
                        UserId = userId,
                        VoteValue = dto.VoteValue
                    };
                    await _uow.IncidentVotes.AddAsync(newVote);
                }

                await _uow.CompleteAsync();
                return Ok(new { Message = "Vote recorded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error casting vote");
                return StatusCode(500, "Internal server error.");
            }
        }

        //GET a Incident of total votes and current user of vote status
        [AllowAnonymous]
        [HttpGet("incident/{incidentId}")]
        public async Task<ActionResult<VoteStatusDto>> GetIncidentVotes(Guid incidentId)
        {
            try
            {
                var votes = await _uow.IncidentVotes.FindAsync(v => v.IncidentId == incidentId);

                int totalVotes = votes.Sum(v => v.VoteValue);
                int? userVoteValue = null;
                
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdString))
                {
                    var userId = Guid.Parse(userIdString);
                    userVoteValue = votes.FirstOrDefault(v => v.UserId == userId)?.VoteValue;
                }

                return Ok(new VoteStatusDto(totalVotes, userVoteValue));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching votes for incident {incidentId}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}