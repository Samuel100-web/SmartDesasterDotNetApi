using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.Entities
{
    public class DonationItem : BaseEntity
    {
        public string ItemName { get; set; } = string.Empty; // "Winter Jackets", "Panadol"
        public Guid CategoryId { get; set; } // Link to Category (e.g., Food, Medicine)
        public virtual DonationCategory Category { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; // Soft Delete
    }
}
