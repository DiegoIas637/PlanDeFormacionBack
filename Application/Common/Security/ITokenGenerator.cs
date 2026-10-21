using Domain.Entities;

namespace Application.Common.Security
{
    public interface ITokenGenerator
    {
        GeneratedToken GenerateToken(User user);
    }

    public record GeneratedToken(string Value, DateTime ExpiresAt);
}
