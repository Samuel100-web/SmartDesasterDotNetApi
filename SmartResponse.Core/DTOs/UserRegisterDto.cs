namespace SmartResponse.Core.DTOs
{
    public record UserRegisterDto(
        string FullName,
        string Email,
        string Password,
        string PhoneNumber,
        string Address,
       
        string? CNIC,
        string? BloodGroup,
        string? EmergencyContact,
        
        bool IsNgoRep,
        Guid? NgoId,
        string? Designation
    );
}