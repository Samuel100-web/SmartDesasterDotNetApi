namespace SmartResponse.Core.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Admin, Volunteer, Public
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
