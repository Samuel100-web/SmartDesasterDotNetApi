using SmartResponse.Core.Enums;

namespace SmartResponse.Core.DTOs
{
    // Patch update for Incident Status (Volunteer action)
    public record IncidentStatusPatchDto(IncidentStatus NewStatus, string Remarks);
}
