using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ApiClient.Auth;

public class AppTokenProvider : ITokenProvider
{
    public Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        // todo. move values into config
        var claims = new HashSet<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, "test"),
                new(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Aud, "test")
            };

        byte[] symmetricKey = Convert.FromBase64String("gTHIcLUuAMu/j5JHaN4WjRD7LABfDtT4iYCFzGJ6b2dHCkx9o4WOlPH8PP3vnKZWQ1YhoDF4A/GcuqTfK0hor/2jQyrtP2RM55bciN8/fONfBJBuvsuV8N1gxX+3mxbS");

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = "test",
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
        };

        JwtSecurityTokenHandler tokenHandler = new();

        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string token = tokenHandler.WriteToken(securityToken);

        return Task.FromResult(token);
    }
}
