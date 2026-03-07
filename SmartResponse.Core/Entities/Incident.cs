using SmartResponse.Core.Enums;

namespace SmartResponse.Core.Entities
{
    public class Incident : BaseEntity
    {
        public decimal? ReporterLiveLong;
        public decimal? ReporterLiveLat;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SeverityLevel Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public decimal TargetLat { get; set; }
        public decimal TargetLong { get; set; }
        public string DataSource { get; set; } = "Manual";

        public Guid IncidentTypeId { get; set; }
        public virtual IncidentType IncidentType { get; set; } = null!;

        public Guid? ReporterId { get; set; } // Nullable for External APIs
        public virtual User? Reporter { get; set; }

        public virtual ICollection<IncidentMedia> Media { get; set; } = new List<IncidentMedia>();
        public virtual ICollection<IncidentVerification> Verifications { get; set; } = new List<IncidentVerification>();
        public virtual ICollection<IncidentVote> Votes { get; set; } = new List<IncidentVote>();
    }
}
