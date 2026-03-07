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
    public class DonationSetupsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public DonationSetupsController(IUnitOfWork uow)
        {
            _uow = uow;
        }        
          
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _uow.DonationCategories.FindAsync(c => !c.IsDeleted);
            var result = categories.Select(c => new CategoryResponseDto(c.Id, c.Name, c.IsActive));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            var category = new DonationCategory { Name = dto.Name, IsActive = true, IsDeleted = false };
            await _uow.DonationCategories.AddAsync(category);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Category created successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto dto)
        {
            var category = await _uow.DonationCategories.GetByIdAsync(id);
            if (category == null || category.IsDeleted) return NotFound("Category not found");

            category.Name = dto.Name;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            _uow.DonationCategories.Update(category);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Category updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _uow.DonationCategories.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.IsDeleted = true;
            _uow.DonationCategories.Update(category);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Category deleted securely" });
        }
        
           
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await _uow.PaymentMethods.FindAsync(p => !p.IsDeleted);
            var result = methods.Select(p => new PaymentMethodResponseDto(p.Id, p.MethodName, p.AccountTitle, p.AccountNumber, p.IsActive));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("payment-methods")]
        public async Task<IActionResult> CreatePaymentMethod(CreatePaymentMethodDto dto)
        {
            var method = new PaymentMethod { MethodName = dto.MethodName, AccountTitle = dto.AccountTitle, AccountNumber = dto.AccountNumber, IsActive = true, IsDeleted = false };
            await _uow.PaymentMethods.AddAsync(method);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Payment method added successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("payment-methods/{id}")]
        public async Task<IActionResult> UpdatePaymentMethod(Guid id, UpdatePaymentMethodDto dto)
        {
            var method = await _uow.PaymentMethods.GetByIdAsync(id);
            if (method == null || method.IsDeleted) return NotFound("Payment method not found");

            method.MethodName = dto.MethodName;
            method.AccountTitle = dto.AccountTitle;
            method.AccountNumber = dto.AccountNumber;
            method.IsActive = dto.IsActive;
            method.UpdatedAt = DateTime.UtcNow;

            _uow.PaymentMethods.Update(method);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Payment method updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("payment-methods/{id}")]
        public async Task<IActionResult> DeletePaymentMethod(Guid id)
        {
            var method = await _uow.PaymentMethods.GetByIdAsync(id);
            if (method == null) return NotFound();

            method.IsDeleted = true;
            _uow.PaymentMethods.Update(method);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Payment method deleted securely" });
        }
        
        // DONATION ITEMS        
        [HttpGet("items")]
        public async Task<IActionResult> GetItems()
        {
            // Includes Category to get Category Name
            var items = await _uow.DonationItems.FindAsync(i => !i.IsDeleted, i => i.Category);
            var result = items.Select(i => new DonationItemResponseDto(i.Id, i.ItemName, i.CategoryId, i.Category.Name, i.IsActive));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("items")]
        public async Task<IActionResult> CreateItem(CreateDonationItemDto dto)
        {
            var item = new DonationItem { ItemName = dto.ItemName, CategoryId = dto.CategoryId, IsActive = true, IsDeleted = false };
            await _uow.DonationItems.AddAsync(item);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Item added successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, UpdateDonationItemDto dto)
        {
            var item = await _uow.DonationItems.GetByIdAsync(id);
            if (item == null || item.IsDeleted) return NotFound("Item not found");

            item.ItemName = dto.ItemName;
            item.CategoryId = dto.CategoryId;
            item.IsActive = dto.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            _uow.DonationItems.Update(item);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Item updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _uow.DonationItems.GetByIdAsync(id);
            if (item == null) return NotFound();

            item.IsDeleted = true; // SOFT DELETE
            _uow.DonationItems.Update(item);
            await _uow.CompleteAsync();
            return Ok(new { Message = "Item deleted securely" });
        }
    }
}