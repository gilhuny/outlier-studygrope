using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudyGroup.Api.Common.Dtos.Auth;
using StudyGroup.Api.Common.Settings.Jwt;
using StudyGroup.Api.Data.Entities.MainEntities;

namespace StudyGroup.Api.Services.Auth;

public class JwtService(IConfiguration configuration)
{
    private readonly JwtSetting _jwtSetting = configuration.GetSection("JwtSettings").Get<JwtSetting>()!;
 
    public TokenDto GenerateToken(User user, bool populateExp)
    {
        var key = Encoding.UTF32.GetBytes(_jwtSetting.Key);
        var signingKey = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
 
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role?.FullName!),
            new("role_code", user.RoleCode.ToString())
        };
 
        var token = new JwtSecurityToken(
            issuer: _jwtSetting.Issuer,
            audience: _jwtSetting.Audience,
            signingCredentials: signingKey,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1));
 
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
 
        if (populateExp)
        {
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpireTime = DateTimeOffset.UtcNow.AddDays(7);
        }
 
        return new TokenDto(AccessToken: accessToken, RefreshToken: user.RefreshToken!);
    }
 
    public Tuple<bool, string?> ValidateAndGetUser(string accessToken)
    {
        var key = Encoding.UTF32.GetBytes(_jwtSetting.Key);
 
        var options = new TokenValidationParameters
        {
            ValidIssuer = _jwtSetting.Issuer,
            ValidateIssuer = true,
            ValidAudience = _jwtSetting.Audience,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false
        };
 
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, options, out var securityToken);
 
            var jwtToken = securityToken as JwtSecurityToken;
            if (jwtToken == null ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return new(false, null);
 
            var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return new(false, null);
 
            return new(true, username);
        }
        catch
        {
            return new(false, null);
        }
    }
 
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}