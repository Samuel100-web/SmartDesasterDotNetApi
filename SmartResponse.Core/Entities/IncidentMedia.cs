namespace SmartResponse.Core.Entities
{
    public class IncidentMedia : BaseEntity
    {
        public Guid IncidentId { get; set; }
        public virtual Incident Incident { get; set; } = null!;
        public string Url { get; set; } = string.Empty;
        public string MediaType { get; set; } = "Image";
    }
}
