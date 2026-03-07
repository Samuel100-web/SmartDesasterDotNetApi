using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record UserProfileUpdateDto(
        string FullName,
        string PhoneNumber,
        string Address,
        string EmergencyContact,
        string BloodGroup,
        string Qualifications,
        string Bio,
        string Skills
    );
}
