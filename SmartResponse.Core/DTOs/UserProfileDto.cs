using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record UserProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string CNIC,
    string Address,
    string BloodGroup,
    string EmergencyContact,
    int TrustScore,
    string RoleName,
    string Qualifications,
    string Bio,
    string Skills,
    string ProfilePictureUrl
);
}
