namespace SmartResponse.Core.DTOs
{
    // Naya verification submit karne ke liye
    public record VerificationCreateDto(Guid IncidentId, string Action, string Remarks);

}
