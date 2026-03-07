using SmartResponse.Core.Enums;

namespace SmartResponse.Core.DTOs
{
    public class IncidentDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LookupDto Type { get; set; } = null!;
        public SeverityLevel Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DataSource { get; set; } = string.Empty;
        public string ReporterName { get; set; } = "Anonymous";
        public int TotalVotes { get; set; } // Sum of Upvotes/Downvotes
        public List<IncidentMediaDto> Media { get; set; } = new();
        public List<VerificationDto> Verifications { get; set; } = new();
    }
}
