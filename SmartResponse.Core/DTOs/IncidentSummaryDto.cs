using SmartResponse.Core.Enums;

namespace SmartResponse.Core.DTOs
{
    public record IncidentSummaryDto(
    Guid Id,
    string Title,
    string TypeName,
    SeverityLevel Severity,
    IncidentStatus Status,
    decimal Lat,
    decimal Long,
    DateTime CreatedAt
);
}
