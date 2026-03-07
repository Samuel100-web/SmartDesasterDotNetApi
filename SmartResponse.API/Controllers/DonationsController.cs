using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;
using System.Security.Claims;

namespace SmartResponse.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DonationsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public DonationsController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        
        [HttpPost]
        public async Task<IActionResult> SubmitDonation(CreateDonationDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                var donation = new Donation
                {
                    UserId = userId,
                    CategoryId = dto.CategoryId,
                    Amount = dto.Amount,
                    PaymentMethodId = dto.PaymentMethodId,
                    TransactionId = dto.TransactionId,
                    ItemId = dto.ItemId,
                    Quantity = dto.Quantity,
                    PickupAddress = dto.PickupAddress,
                    Status = "Pending"
                };

                await _uow.Donations.AddAsync(donation);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Thank you for your generous donation!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
        
        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyDonations()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            var donations = await _uow.Donations.FindAsync(
                d => d.UserId == userId,
                d => d.Category, d => d.PaymentMethod, d => d.Item
            );

            var result = donations.OrderByDescending(d => d.CreatedAt).Select(d => new DonationResponseDto(
                d.Id,
                d.CreatedAt,
                d.NGOName,
                d.Category.Name,
                d.Amount,
                d.PaymentMethod?.MethodName,
                d.TransactionId,
                d.Item?.ItemName,
                d.Quantity,
                d.PickupAddress,
                d.Status
            ));

            return Ok(result);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllDonations()
        {
            var donations = await _uow.Donations.FindAsync(
                d => true,
                d => d.Category, d => d.PaymentMethod, d => d.Item
            );

            var result = donations.OrderByDescending(d => d.CreatedAt).Select(d => new DonationResponseDto(
                d.Id, d.CreatedAt, d.NGOName, d.Category.Name, d.Amount, d.PaymentMethod?.MethodName,
                d.TransactionId, d.Item?.ItemName, d.Quantity, d.PickupAddress, d.Status
            ));

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateDonationStatusDto dto)
        {
            var donation = await _uow.Donations.GetByIdAsync(id);
            if (donation == null) return NotFound();

            donation.Status = dto.Status; // e.g., "Received", "Verified"
            donation.UpdatedAt = DateTime.UtcNow;

            _uow.Donations.Update(donation);
            await _uow.CompleteAsync();

            return Ok(new { Message = "Status updated successfully" });
        }
    }
}