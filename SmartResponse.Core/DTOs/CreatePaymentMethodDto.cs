using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record CreatePaymentMethodDto(string MethodName, string AccountTitle, string AccountNumber);
}
