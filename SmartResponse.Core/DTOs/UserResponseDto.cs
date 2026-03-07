namespace SmartResponse.Core.DTOs
{
    // Login ke baad jo data frontend ko jayega (No Password!)
    public record UserResponseDto(
    Guid Id,
    string FullName,
    string Email,
    string RoleName,
    int TrustScore,
    string PhoneNumber,
    string BloodGroup
);

}
