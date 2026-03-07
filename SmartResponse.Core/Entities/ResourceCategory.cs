namespace SmartResponse.Core.Entities
{
    public class ResourceCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
