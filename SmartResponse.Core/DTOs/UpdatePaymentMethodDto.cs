using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record UpdatePaymentMethodDto(string MethodName, string AccountTitle, string AccountNumber, bool IsActive);
}
