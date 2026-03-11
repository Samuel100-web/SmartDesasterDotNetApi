using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlidersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public SlidersController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sliders = await _uow.HomeSlider.GetAllAsync();
            return Ok(sliders.Where(s => s.IsActive));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] SliderCreateDto dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("No file uploaded");
            
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "sliders");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }

            
            var slider = new HomeSlider
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = $"/uploads/sliders/{fileName}",
                IsActive = true
            };

            await _uow.HomeSlider.AddAsync(slider);
            await _uow.CompleteAsync();

            return Ok(slider);
        }
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] SliderCreateDto dto)
        {
            var slider = await _uow.HomeSlider.GetByIdAsync(id);
            if (slider == null) return NotFound("Slider not found");

            slider.Title = dto.Title;
            slider.Description = dto.Description;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "sliders");
                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                
                slider.ImageUrl = $"/uploads/sliders/{fileName}";
            }        

            _uow.HomeSlider.Update(slider);
            await _uow.CompleteAsync();

            return Ok(slider);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var slider = await _uow.HomeSlider.GetByIdAsync(id);
            if (slider == null) return NotFound();
            _uow.HomeSlider.Delete(slider);
            await _uow.CompleteAsync();
            return Ok();
        }
    }
}
