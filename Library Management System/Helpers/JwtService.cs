using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Library_Management_System.Helpers;

public class JwtService(IConfiguration config)
{
    /// <summary>
    /// Creates a signed JSON Web Token containing the subject and role claims using JWT settings from configuration.
    /// </summary>
    /// <param name="userId">The subject identifier to include in the token's `sub` claim.</param>
    /// <param name="role">The role name to include in the token's role claim.</param>
    /// <returns>A serialized JWT string signed with the configured secret and valid for 4 hours.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configured JWT secret key is missing or shorter than 16 characters.</exception>
    public string GenerateToken(string userId, string role,string userName)
    {
       

        var secretKeyString = config["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKeyString) || secretKeyString.Length < 32)
            throw new InvalidOperationException(
                "JWT SecretKey is missing or too short (minimum 32 characters).");

        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];

        if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            throw new InvalidOperationException(
                "JWT Issuer and Audience are missing or not set.");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKeyString));

        var creds = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier,userId),
            new Claim(ClaimTypes.Name,userName),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}