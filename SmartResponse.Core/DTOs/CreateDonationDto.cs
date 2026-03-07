using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record CreateDonationDto(
        Guid CategoryId,
        decimal? Amount,
        Guid? PaymentMethodId,
        string? TransactionId,
        Guid? ItemId,
        string? Quantity,
        string? PickupAddress
    );
}
