using SmartResponse.Core.Enums;

namespace SmartResponse.Core.DTOs
{
    public record IncidentCreateDto(
    string Title,
    string Description,
    Guid IncidentTypeId,
    SeverityLevel Severity,
    decimal TargetLat,
    decimal TargetLong,
    decimal? ReporterLiveLat,
    decimal? ReporterLiveLong
);
}
