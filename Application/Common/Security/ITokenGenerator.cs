using System.Security.Claims;
using Domain.Entities;

namespace Application.Common.Security
{
    public interface ITokenGenerator
    {
        GeneratedToken GenerateToken(User user);
        int GetUserId(ClaimsPrincipal principal);
    }

    public record GeneratedToken(string Value, DateTime ExpiresAt);
}
