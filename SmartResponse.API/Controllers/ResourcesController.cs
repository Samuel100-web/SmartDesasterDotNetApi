using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    [Authorize] //Default security: Login complsary
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ResourcesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //GET All Resources Public or Volunteers
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceReadDto>>> GetAllResources()
        {            
            var resources = await _uow.Resources.FindAsync(r => true, r => r.Category);

            var result = resources.Select(r => new ResourceReadDto(
                r.Id,
                r.Name,
                r.Category.Name,
                r.AvailableQty,
                r.TotalQty,
                r.Lat,
                r.Long
            ));

            return Ok(result);
        }

        //GET Single Resource Detail
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceReadDto>> GetResourceById(Guid id)
        {
            var resourceList = await _uow.Resources.FindAsync(r => r.Id == id, r => r.Category);
            var resource = resourceList.FirstOrDefault();

            if (resource == null) return NotFound("Resource not found");

            var result = new ResourceReadDto(
                resource.Id,
                resource.Name,
                resource.Category.Name,
                resource.AvailableQty,
                resource.TotalQty,
                resource.Lat,
                resource.Long
            );

            return Ok(result);
        }

        //POST Naya Resource Add Karna (Admin/Volunteer Only)
        [Authorize(Roles = "Admin,Volunteer")]
        [HttpPost]
        public async Task<IActionResult> CreateResource(ResourceCreateUpdateDto dto)
        {            
            if (dto.AvailableQty > dto.TotalQty)
                return BadRequest("Available quantity cannot exceed total quantity.");

            var resource = new Resource
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                TotalQty = dto.TotalQty,
                AvailableQty = dto.AvailableQty,
                Lat = dto.Lat,
                Long = dto.Long
            };

            await _uow.Resources.AddAsync(resource);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Resource added successfully", ResourceId = resource.Id });
        }

        //PUT Pura Resource Update Karna
        [Authorize(Roles = "Admin,Volunteer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(Guid id, ResourceCreateUpdateDto dto)
        {
            var resource = await _uow.Resources.GetByIdAsync(id);
            if (resource == null) return NotFound();

            if (dto.AvailableQty > dto.TotalQty)
                return BadRequest("Available quantity cannot exceed total quantity.");

            resource.Name = dto.Name;
            resource.CategoryId = dto.CategoryId;
            resource.TotalQty = dto.TotalQty;
            resource.AvailableQty = dto.AvailableQty;
            resource.Lat = dto.Lat;
            resource.Long = dto.Long;
            resource.UpdatedAt = DateTime.UtcNow;

            _uow.Resources.Update(resource);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Resource updated successfully" });
        }

        //PATCH Sirf Available Quantity Update Karna (After relief work)
        [Authorize(Roles = "Admin,Volunteer")]
        [HttpPatch("{id}/quantity")]
        public async Task<IActionResult> UpdateQuantity(Guid id, ResourceQuantityPatchDto dto)
        {
            var resource = await _uow.Resources.GetByIdAsync(id);
            if (resource == null) return NotFound();

            if (dto.NewAvailableQty > resource.TotalQty)
                return BadRequest("New available quantity cannot exceed total quantity.");

            resource.AvailableQty = dto.NewAvailableQty;
            resource.UpdatedAt = DateTime.UtcNow;

            _uow.Resources.Update(resource);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Quantity updated successfully" });
        }

        //DELETE Soft Delete Resource
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(Guid id)
        {
            var resource = await _uow.Resources.GetByIdAsync(id);
            if (resource == null) return NotFound();

            _uow.Resources.Delete(resource);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Resource deleted successfully" });
        }
    }
}