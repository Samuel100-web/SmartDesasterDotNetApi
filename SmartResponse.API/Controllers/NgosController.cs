using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NgosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public NgosController(IUnitOfWork uow) { _uow = uow; }
        
        [HttpGet]
        public async Task<IActionResult> GetActiveNgos()
        {
            var ngos = await _uow.Ngos.FindAsync(n => !n.IsDeleted && n.IsActive);
            var result = ngos.Select(n => new NgoResponseDto(n.Id, n.Name, n.RegistrationNumber, n.ContactPhone, n.HeadOfficeAddress, n.IsActive));
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateNgo(CreateNgoDto dto)
        {
            var ngo = new Ngo { Name = dto.Name, RegistrationNumber = dto.RegistrationNumber, ContactPhone = dto.ContactPhone, HeadOfficeAddress = dto.HeadOfficeAddress, IsActive = true, IsDeleted = false };
            await _uow.Ngos.AddAsync(ngo);
            await _uow.CompleteAsync();
            return Ok(new { Message = "NGO added successfully" });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNgo(Guid id, UpdateNgoDto dto)
        {
            var ngo = await _uow.Ngos.GetByIdAsync(id);
            if (ngo == null || ngo.IsDeleted) return NotFound("NGO not found");

            ngo.Name = dto.Name;
            ngo.RegistrationNumber = dto.RegistrationNumber;
            ngo.ContactPhone = dto.ContactPhone;
            ngo.HeadOfficeAddress = dto.HeadOfficeAddress;
            ngo.IsActive = dto.IsActive;
            ngo.UpdatedAt = DateTime.UtcNow;

            _uow.Ngos.Update(ngo);
            await _uow.CompleteAsync();
            return Ok(new { Message = "NGO updated successfully" });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNgo(Guid id)
        {
            var ngo = await _uow.Ngos.GetByIdAsync(id);
            if (ngo == null) return NotFound();

            ngo.IsDeleted = true;
            _uow.Ngos.Update(ngo);
            await _uow.CompleteAsync();
            return Ok(new { Message = "NGO deleted securely" });
        }
    }
}