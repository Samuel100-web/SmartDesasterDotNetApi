using SmartResponse.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record IncidentUpdateDto(
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
