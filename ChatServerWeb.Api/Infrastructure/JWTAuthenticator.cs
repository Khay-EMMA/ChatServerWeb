using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatServerWeb.Api.Infrastructure.Configuration;
using ChatServerWeb.Model.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ChatServerWeb.Api.Infrastructure
{
    public class JWTAUthenticator
    {
        readonly JWTConfig jwtConfig;
        public JWTAUthenticator(JWTConfig _jwtConfig)
        {
            jwtConfig = _jwtConfig;
        }

        public JWT GenerateJwtToken(ApplicationUser user)
        {
            var currentDateTime = DateTime.Now;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.ServerSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = currentDateTime.AddMinutes(Convert.ToDouble(jwtConfig.ExpiresIn));

            var token = new JwtSecurityToken(
                jwtConfig.Issuer,
                jwtConfig.Audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new JWT
            {
                AccessToken = encodedJwt,
                Issued = currentDateTime,
                Expires = expires,
            };
        }


    }
}


public class JWT
{
    public string AccessToken { get; set; }
    public DateTime Issued { get; set; }
    public DateTime Expires { get; set; }
}
