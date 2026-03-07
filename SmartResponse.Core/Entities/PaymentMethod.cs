using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public string MethodName { get; set; } = string.Empty; // "Bank Transfer", "JazzCash"
        public string AccountTitle { get; set; } = string.Empty; // "Smart Response NGO"
        public string AccountNumber { get; set; } = string.Empty; // "03001234567" or IBAN
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; // Soft Delete
    }
}
