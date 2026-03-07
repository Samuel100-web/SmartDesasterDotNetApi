using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record DonationResponseDto(
        Guid Id,
        DateTime CreatedAt,
        string NGOName,
        string CategoryName,
        decimal? Amount,
        string? PaymentMethodName,
        string? TransactionId,
        string? ItemName,
        string? Quantity,
        string? PickupAddress,
        string Status
    );
}
