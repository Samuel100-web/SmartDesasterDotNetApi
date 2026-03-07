using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceCategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ResourceCategoriesController> _logger;

        public ResourceCategoriesController(IUnitOfWork uow, ILogger<ResourceCategoriesController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        //GET All Categories (Publicly available for dropdowns)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceCategoryDto>>> GetAll()
        {
            try
            {
                var categories = await _uow.ResourceCategories.GetAllAsync();
                var result = categories.Select(c => new ResourceCategoryDto(c.Id, c.Name));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching resource categories");
                return StatusCode(500, "Internal server error occurred.");
            }
        }

        //GET By ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceCategoryDto>> GetById(Guid id)
        {
            try
            {
                var category = await _uow.ResourceCategories.GetByIdAsync(id);
                if (category == null) return NotFound("Category not found.");

                return Ok(new ResourceCategoryDto(category.Id, category.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching category {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        //POST Create Category (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ResourceCategoryCreateUpdateDto dto)
        {
            try
            {
                var category = new ResourceCategory
                {
                    Name = dto.Name
                };

                await _uow.ResourceCategories.AddAsync(category);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Category created successfully", Id = category.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating resource category");
                return StatusCode(500, "Could not save category.");
            }
        }

        //PUT Update Category (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceCategoryCreateUpdateDto dto)
        {
            try
            {
                var category = await _uow.ResourceCategories.GetByIdAsync(id);
                if (category == null) return NotFound();

                category.Name = dto.Name;
                category.UpdatedAt = DateTime.UtcNow;

                _uow.ResourceCategories.Update(category);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Category updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category {id}");
                return StatusCode(500, "Update failed.");
            }
        }

        //DELETE Soft Delete (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var category = await _uow.ResourceCategories.GetByIdAsync(id);
                if (category == null) return NotFound();
                
                var resources = await _uow.Resources.FindAsync(r => r.CategoryId == id);
                if (resources.Any())
                {
                    return BadRequest("Cannot delete category that contains resources.");
                }

                _uow.ResourceCategories.Delete(category);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Category soft-deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category {id}");
                return StatusCode(500, "Delete operation failed.");
            }
        }
    }
}