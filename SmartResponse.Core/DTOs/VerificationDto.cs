namespace SmartResponse.Core.DTOs
{
    public record VerificationDto(Guid Id, string VolunteerName, string Action, string Remarks, DateTime CreatedAt);
}
