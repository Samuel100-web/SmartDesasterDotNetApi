using SmartResponse.Core.Entities;

namespace SmartResponse.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, string roleName);
    }
}
