using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Security.Api.Infrastructure.Authentication;

public static class JwtSigningKeyFactory
{
    public static SymmetricSecurityKey Create(string secretKey)
        => new(Encoding.UTF8.GetBytes(secretKey));
}