using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TechChallenge.Domain.Models;

namespace TechChallenge.Security;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string Generate(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(5),
        };
        
        var token = handler.CreateToken(tokenDescriptor);
        
        return handler.WriteToken(token);
    }
}