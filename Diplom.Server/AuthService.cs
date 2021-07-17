using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Diplom.Common.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Diplom.Server
{
    public static class AuthService
    {
        public const string Issuer = "MyAuthServer"; // издатель токена
        public const string Audience = "MyAuthClient"; // потребитель токена

        private const string
                Key = "mysupersecret_secretkey!123"; // ключ для шифрации хранит ключ, который будет применяться для создания токена

        private const int LifetimeInMinutes = 1440; // время жизни токена - 360 минут (6 часов)

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        }

        public static string GenerateToken(SiteUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(LifetimeInMinutes);

            var token = new JwtSecurityToken(
                                             Issuer,
                                             Audience,
                                             notBefore: DateTime.UtcNow,
                                             claims: claims,
                                             expires: expires,
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}