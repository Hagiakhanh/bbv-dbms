using System.Security.Claims;
using OnlineStore.Entities;

namespace OnlineStore.Services.Tokens
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
