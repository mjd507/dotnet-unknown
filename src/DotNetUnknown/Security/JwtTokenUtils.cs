using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DotNetUnknown.Security;

public sealed class JwtTokenUtils(IOptions<JwtSettings> jwtSettingsOption)
{
    private readonly JwtSettings _jwtSettings = jwtSettingsOption.Value;

    // Value is set once and stays the same for the app's lifetime

    /**
     * generate JWT token for client use.
     * usually called after a successful login. (in an authorization server)
     */
    public string GenerateToken(UserInfo userInfo)
    {
        var jwtKey = Encoding.ASCII.GetBytes(_jwtSettings.Key);
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
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}