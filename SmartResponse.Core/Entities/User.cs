namespace SmartResponse.Core.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        public string? CNIC { get; set; } = string.Empty;
        public string? BloodGroup { get; set; } = string.Empty;
        public string? EmergencyContact { get; set; } = string.Empty;

        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Qualifications { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;

        public int TrustScore { get; set; } = 0;
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
        
        public Guid? NgoId { get; set; }
        public virtual Ngo? Ngo { get; set; }
        public string? Designation { get; set; } = string.Empty;
       
        public virtual ICollection<Incident> ReportedIncidents { get; set; } = new List<Incident>();
        public virtual ICollection<IncidentVerification> Verifications { get; set; } = new List<IncidentVerification>();
    }
}