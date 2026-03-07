using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentTypesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<IncidentTypesController> _logger;

        public IncidentTypesController(IUnitOfWork uow, ILogger<IncidentTypesController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        //GET All Types (Public dropdowns ke liye lazmi hai)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncidentTypeDto>>> GetAll()
        {
            try
            {
                var types = await _uow.IncidentTypes.GetAllAsync();
                var result = types.Select(t => new IncidentTypeDto(t.Id, t.Name, t.Icon));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching incident types");
                return StatusCode(500, "Internal server error occurred while fetching data.");
            }
        }

        //POST Naya Type Add Karna
        [HttpPost]
        public async Task<IActionResult> Create(IncidentTypeCreateUpdateDto dto)
        {
            try
            {
                var incidentType = new IncidentType
                {
                    Name = dto.Name,
                    Icon = dto.Icon
                };

                await _uow.IncidentTypes.AddAsync(incidentType);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Incident Type created", Id = incidentType.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating incident type");
                return StatusCode(500, "Error saving data to database.");
            }
        }

        //PUT Type Update Karna
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, IncidentTypeCreateUpdateDto dto)
        {
            try
            {
                var type = await _uow.IncidentTypes.GetByIdAsync(id);
                if (type == null) return NotFound();

                type.Name = dto.Name;
                type.Icon = dto.Icon;
                type.UpdatedAt = DateTime.UtcNow;

                _uow.IncidentTypes.Update(type);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating incident type {id}");
                return StatusCode(500, "Update failed.");
            }
        }

        //GET By ID Specific Type nikalne ke liye
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentTypeDto>> GetById(Guid id)
        {
            try
            {                
                var type = await _uow.IncidentTypes.GetByIdAsync(id);

                if (type == null)
                {
                    _logger.LogWarning($"Incident type with ID {id} not found.");
                    return NotFound(new { Message = "Incident type not found." });
                }
                
                var result = new IncidentTypeDto(type.Id, type.Name, type.Icon);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching incident type with ID: {id}");
                return StatusCode(500, "Internal server error occurred while retrieving the data.");
            }
        }

        //DELETE Soft Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var type = await _uow.IncidentTypes.GetByIdAsync(id);
                if (type == null) return NotFound();                

                _uow.IncidentTypes.Delete(type);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Soft deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting incident type {id}");
                return StatusCode(500, "Delete operation failed.");
            }
        }
    }
}