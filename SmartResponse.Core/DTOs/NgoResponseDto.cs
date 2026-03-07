using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record NgoResponseDto(Guid Id, string Name, string RegistrationNumber, string ContactPhone, string HeadOfficeAddress, bool IsActive);
}
