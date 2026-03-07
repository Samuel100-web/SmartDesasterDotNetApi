using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.Entities
{
    public class DonationCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., "Money", "Food", "Medical"
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; // Soft Delete ke liye
    }
}
