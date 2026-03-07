namespace SmartResponse.Core.Entities
{
    public class Resource : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int TotalQty { get; set; }
        public int AvailableQty { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }

        public Guid CategoryId { get; set; }
        public virtual ResourceCategory Category { get; set; } = null!;
    }
}
