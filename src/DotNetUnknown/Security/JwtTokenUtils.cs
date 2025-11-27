using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DotNetUnknown.Security;

public sealed class JwtTokenUtils(IConfiguration configuration)
{
    /**
     * generate JWT token for client use.
     * usually called after a successful login. (in an authorization server)
     */
    public string GenerateToken(UserInfo userInfo)
    {
        var jwtSettings = configuration.GetRequiredSection("JwtSettings");
#nullable disable
        var jwtKey = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
#nullable enable
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userInfo.UserId),
            new(ClaimTypes.Name, userInfo.Username),
            new(ClaimTypes.Email, userInfo.Email),
            new("employee_status", userInfo.Status)
        };
        claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}