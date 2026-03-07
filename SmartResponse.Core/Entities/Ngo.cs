namespace SmartResponse.Core.Entities
{
    public class Ngo : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string HeadOfficeAddress { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        
        public virtual ICollection<User> Representatives { get; set; } = new List<User>();
    }
}
