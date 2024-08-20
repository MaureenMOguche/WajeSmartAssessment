using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WajeSmartAssessment.Application.Config;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Implementations;
public class TokenHelper(IOptions<JwtSettings> jwtSettings,
    IHttpContextAccessor contextAccessor) : ITokenHelper
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    public string GenerateLoginToken(AppUser user)
    {
        var role = user.Role.ToString();

        var claims = new Claim[]
        {
            new(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
        signingCredentials: signingCredentials,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks;

        SetCookie("refreshToken", refreshToken);

        return tokenString;
    }


    private void SetCookie(string key, string value)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Ensure to use HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.CookieDurationInMinutes)
        };

        contextAccessor?.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
    }

}
