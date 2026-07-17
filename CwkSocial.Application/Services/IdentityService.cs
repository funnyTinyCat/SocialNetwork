using Cwk.Domain.Aggregates.UserProfileAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CwkSocial.Application.Services
{
    public class IdentityService
    {
        private readonly IConfiguration _config;
        private readonly byte[] _key;

        public IdentityService(IConfiguration config)
        {
            _config = config;
            _key = Encoding.ASCII.GetBytes(_config["JwtSettings:SigningKey"]!);
        }

        public JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        
        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            
            var tokenDescriptor = GetTokenDescriptor(identity);

            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public string WriteToken(SecurityToken token)
        {
            return tokenHandler.WriteToken(token);
        }

        private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
        {
            return new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(2),
                Audience = _config["JwtSettings:Audience"],
                Issuer = _config["JwtSettings:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key),
                        SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
