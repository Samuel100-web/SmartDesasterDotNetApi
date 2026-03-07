using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Interfaces;
using System.Security.Claims;

namespace SmartResponse.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UsersController> _logger;
        private readonly IWebHostEnvironment _env;

        public UsersController(IUnitOfWork uow, ILogger<UsersController> logger, IWebHostEnvironment env)
        {
            _uow = uow;
            _logger = logger;
            _env = env;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            try
            {                
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                               ?? User.FindFirst("nameid");

                if (userIdClaim == null) return Unauthorized("User ID claim not found in token");

                var userId = Guid.Parse(userIdClaim.Value);
                
                var userList = await _uow.Users.FindAsync(u => u.Id == userId, u => u.Role);
                var user = userList.FirstOrDefault();

                if (user == null) return NotFound("User not found in DB");
                
                return Ok(new UserProfileDto(
                    user.Id, user.FullName, user.Email, user.PhoneNumber, user.CNIC,
                    user.Address, user.BloodGroup, user.EmergencyContact, user.TrustScore,
                    user.Role.Name, user.Qualifications, user.Bio, user.Skills, user.ProfilePictureUrl
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile fetch error");
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
                var user = await _uow.Users.GetByIdAsync(userId);

                if (user == null) return NotFound();

                user.FullName = dto.FullName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address;
                user.EmergencyContact = dto.EmergencyContact;
                user.BloodGroup = dto.BloodGroup;
                user.Qualifications = dto.Qualifications;
                user.Bio = dto.Bio;
                user.Skills = dto.Skills;
                user.UpdatedAt = DateTime.UtcNow;

                _uow.Users.Update(user);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Text profile updated" });
            }
            catch (Exception ex) { return StatusCode(500, "Update failed"); }
        }

        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture(IFormFile file)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
                var user = await _uow.Users.GetByIdAsync(userId);
                if (user == null) return NotFound();
                
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }
                
                string folder = Path.Combine(_env.WebRootPath, "uploads/profiles");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                user.ProfilePictureUrl = "/uploads/profiles/" + fileName;
                _uow.Users.Update(user);
                await _uow.CompleteAsync();

                return Ok(new { Url = user.ProfilePictureUrl });
            }
            catch (Exception ex) { return StatusCode(500, "Upload failed"); }
        }

            
        [Authorize(Roles = "Admin")]
        [HttpPatch("{userId}/trust-score")]
        public async Task<IActionResult> UpdateTrustScore(Guid userId, TrustScoreUpdateDto dto)
        {
            try
            {
                var user = await _uow.Users.GetByIdAsync(userId);
                if (user == null) return NotFound("User not found.");

                user.TrustScore = dto.NewScore;
                user.UpdatedAt = DateTime.UtcNow;

                _uow.Users.Update(user);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Trust score updated by admin", NewScore = user.TrustScore });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating trust score for user {userId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [Authorize(Roles = "Admin,Volunteer")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(Guid userId)
        {
            try
            {
                var userList = await _uow.Users.FindAsync(u => u.Id == userId, u => u.Role);
                var user = userList.FirstOrDefault();

                if (user == null) return NotFound();
                
                return Ok(new UserProfileDto(
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.CNIC,
                    user.Address,
                    user.BloodGroup,
                    user.EmergencyContact,
                    user.TrustScore,
                    user.Role.Name,
                    user.Qualifications,
                    user.Bio,
                    user.Skills,
                    user.ProfilePictureUrl
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching profile for {userId}");
                return StatusCode(500, "Internal server error.");
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var user = await _uow.Users.GetByIdAsync(userId);
                if (user == null) return NotFound("User not found.");
                
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                if (userId == currentUserId) return BadRequest("You cannot delete your own admin account.");
                
                _uow.Users.Delete(user);
                await _uow.CompleteAsync();

                return Ok(new { Message = "User has been soft-deleted and can no longer login." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}