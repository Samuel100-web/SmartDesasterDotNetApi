using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record CreateDonationItemDto(string ItemName, Guid CategoryId);
}
