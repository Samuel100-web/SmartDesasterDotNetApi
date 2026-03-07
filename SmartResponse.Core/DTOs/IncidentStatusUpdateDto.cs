using SmartResponse.Core.Enums;

namespace SmartResponse.Core.DTOs
{
    public class IncidentStatusUpdateDto
    {
        public IncidentStatus NewStatus { get; set; }
        public string Remarks { get; set; } = string.Empty; // Kyun change kiya?
    }
}
