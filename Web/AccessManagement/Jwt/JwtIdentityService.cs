using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Aidan.Core.Patterns;
using Aidan.Web.AccessManagement.Services;
using Aidan.Web.AccessManagement.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Aidan.Web.AccessManagement.Jwt.Services;

public class JwtIdentityService : IIdentityService
{
    public IOperationResult<IIdentity?> GetIdentity(HttpContext httpContext)
    {
        var token = httpContext.GetBearerToken();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var identity = new JwtIdentity(
            identifier: jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
            permissions: jwtToken.Claims.Where(c => c.Type == "permission").Select(c => new JwtIdentityPermission(new[] { c.Value })).ToArray()
        );

        return new OperationResult<IIdentity?>(data: identity);
    }

    public JwtToken CreateToken(JwtIdentity identity)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identity.Identifier ?? string.Empty)
            };
        
        foreach (var permission in identity.Permissions)
        {

            claims.AddRange(permission.Segments.Select(s => new Claim("permission", s)));
        }

        claims.AddRange(permissions.SelectMany(p => p.Segments.Select(s => new Claim("permission", s))));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new JwtToken(tokenString);
    }

    public string EncryptToken(JwtToken token)
    {
        throw new NotImplementedException();
    }
}

public class JwtIdentity : IIdentity
{
    public string? Identifier { get; }
    public JwtIdentityPermission[] Permissions { get; }

    [JsonConstructor]
    public JwtIdentity(string? identifier, JwtIdentityPermission[] permissions)
    {
        Identifier = identifier;
        Permissions = permissions;
    }

    public IEnumerable<IIdentityPermission> GetPermissions()
    {
        return Permissions;
    }
}

public class JwtToken : IWebToken
{
    public DateTime CreatedAt { get; }
    public TimeSpan Lifetime { get; }
    public string? Payload { get; }

    [JsonConstructor]
    public JwtToken(
        string payload, 
        TimeSpan lifetime, 
        DateTime? createdAt = null)
    {
        CreatedAt = createdAt ?? DateTime.UtcNow;
        Lifetime = lifetime;
        Payload = payload;
    }

    public TimeSpan GetLifetime()
    {
        return DateTime.UtcNow - CreatedAt;
    }

    public string? GetPayload()
    {
        return Payload;
    }
}

public class JwtIdentityPermission : IIdentityPermission
{
    public string[] Segments { get; }

    [JsonConstructor]
    public JwtIdentityPermission(string[] segments)
    {
        Segments = segments;
    }

    public IEnumerable<string> GetSegments()
    {
        return Segments;
    }
}

/*
 * Builders
 */

public class JwtIdentityBuilder : IBuilder<JwtIdentity>
{
    private string? Identifier { get; set; }
    private List<JwtIdentityPermission> Permissions { get; } = new();

    public JwtIdentityBuilder SetIdentifier(string? identifier)
    {
        Identifier = identifier;
        return this;
    }

    public JwtIdentityBuilder AddPermission(string permission)
    {
        Permissions.Add(new JwtIdentityPermission(permission.Split('.')));
        return this;
    }

    public JwtIdentity Build()
    {
        return new JwtIdentity(Identifier, Permissions.ToArray());
    }
}
