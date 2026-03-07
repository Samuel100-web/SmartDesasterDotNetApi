using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record CreateNgoDto(string Name, string RegistrationNumber, string ContactPhone, string HeadOfficeAddress);
}
