using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record CategoryResponseDto(Guid Id, string Name, bool IsActive);
}
