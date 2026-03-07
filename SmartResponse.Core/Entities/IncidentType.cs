namespace SmartResponse.Core.Entities
{
    public class IncidentType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}
