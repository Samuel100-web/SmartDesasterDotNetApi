using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.Entities
{
    public class Donation : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        // Kis NGO ko donate kiya? (Agar future me multiple NGOs hon)
        public string NGOName { get; set; } = "Smart Response Foundation";

        // 1. Dynamic Category (Money ya Items)
        public Guid CategoryId { get; set; }
        public virtual DonationCategory Category { get; set; } = null!;

        // --- MONEY DONATION FIELDS ---
        public decimal? Amount { get; set; }
        public Guid? PaymentMethodId { get; set; } // Nullable, kyunke items donate karte waqt zaroorat nahi
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public string? TransactionId { get; set; } = string.Empty;

        // --- ITEMS DONATION FIELDS ---
        public Guid? ItemId { get; set; } // Nullable, kyunke paisay donate karte waqt zaroorat nahi
        public virtual DonationItem? Item { get; set; }

        public string? Quantity { get; set; } = string.Empty; // e.g., "5 Kg", "10 Boxes"
        public string? PickupAddress { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending"; // Pending, Received, Verified
    }
}
