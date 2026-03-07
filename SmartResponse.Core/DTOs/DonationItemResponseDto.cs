using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record DonationItemResponseDto(Guid Id, string ItemName, Guid CategoryId, string CategoryName, bool IsActive);
}
