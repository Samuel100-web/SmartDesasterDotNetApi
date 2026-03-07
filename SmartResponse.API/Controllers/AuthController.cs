using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;

        public AuthController(IUnitOfWork uow, ITokenService tokenService)
        {
            _uow = uow;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto dto)
        {            
            var users = await _uow.Users.FindAsync(u => u.Email == dto.Email.ToLower());
            if (users.Any()) return BadRequest("Email is already registered");
            
            string roleName = dto.IsNgoRep ? "NGO" : "Public";
            var roles = await _uow.Roles.FindAsync(r => r.Name == roleName);
            var assignedRole = roles.FirstOrDefault();

            if (assignedRole == null) return BadRequest($"{roleName} role not found in database.");
            
            if (dto.IsNgoRep)
            {
                if (dto.NgoId == null) return BadRequest("Please select an NGO.");
                var ngo = await _uow.Ngos.GetByIdAsync(dto.NgoId.Value);
                if (ngo == null || ngo.IsDeleted || !ngo.IsActive) return BadRequest("Selected NGO is invalid or inactive.");
            }
            else
            {                
                if (!string.IsNullOrEmpty(dto.CNIC))
                {
                    var cnicExists = await _uow.Users.FindAsync(u => u.CNIC == dto.CNIC);
                    if (cnicExists.Any()) return BadRequest("CNIC is already registered");
                }
            }
            
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                
                CNIC = dto.CNIC ?? string.Empty,
                BloodGroup = dto.BloodGroup ?? string.Empty,
                EmergencyContact = dto.EmergencyContact ?? string.Empty,
                
                NgoId = dto.IsNgoRep ? dto.NgoId : null,
                Designation = dto.IsNgoRep ? dto.Designation : string.Empty,

                RoleId = assignedRole.Id,
                TrustScore = 0
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();

            return new UserResponseDto(
                user.Id,
                user.FullName,
                user.Email,
                assignedRole.Name,
                user.TrustScore,
                user.PhoneNumber,
                user.BloodGroup ?? ""
            );
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto dto)
        {
            var userList = await _uow.Users.FindAsync(u => u.Email == dto.Email.ToLower(), u => u.Role);
            var user = userList.FirstOrDefault();

            if (user == null) return Unauthorized("Please provide Invalid Email or Password");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Please provide Invalid Email or Password");

            return _tokenService.CreateToken(user, user.Role.Name);
        }
    }
}