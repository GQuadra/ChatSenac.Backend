using System.Security.Claims;
using System.Text;
using ChatSENAC.Backend.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ChatSENAC.Backend.Services;

public class TokenServico
{
    public static (string Token, DateTime ExpiresAt) CriarToken(Usuario usuario)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(240);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenKey = Environment.GetEnvironmentVariable("JWT_KEY") ??
                       throw new InvalidOperationException("A chave secreta do token não foi configurada");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        return (token, expiresAt);
    }
}