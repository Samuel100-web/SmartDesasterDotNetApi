namespace SmartResponse.Core.Entities
{
    public class IncidentVerification : BaseEntity
    {
        public Guid IncidentId { get; set; }
        public virtual Incident Incident { get; set; } = null!;
        public Guid VolunteerId { get; set; }
        public virtual User Volunteer { get; set; } = null!;
        public string Action { get; set; } = string.Empty; // Verified/Rejected
        public string Remarks { get; set; } = string.Empty;
    }
}
