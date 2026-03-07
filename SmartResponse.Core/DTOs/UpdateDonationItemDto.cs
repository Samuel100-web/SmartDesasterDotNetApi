using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record UpdateDonationItemDto(string ItemName, Guid CategoryId, bool IsActive);

}
