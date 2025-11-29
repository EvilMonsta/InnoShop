using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace InnoShop.Users.Infrastructure.Auth;


public class JwtTokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;


    public JwtTokenService(IConfiguration cfg)
    {
        _issuer = cfg["Jwt:Issuer"] ?? "innoshop";
        _audience = cfg["Jwt:Audience"] ?? "innoshop.clients";
        _key = cfg["JwtSigningKey"] ?? cfg["Jwt:SigningKey"]
                   ?? throw new InvalidOperationException("Jwt signing key is required");
    }


    public string GenerateAccessToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
{
new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
new(JwtRegisteredClaimNames.Email, user.Email),
new(ClaimTypes.Name, user.Name),
new(ClaimTypes.Role, user.Role.ToString())
};
        var token = new JwtSecurityToken(_issuer, _audience, claims,
        expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials);
        return handler.WriteToken(token);
    }
}