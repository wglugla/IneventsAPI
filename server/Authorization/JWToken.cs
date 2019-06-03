using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Authorization
{
    public class JWToken
    {
        private IConfiguration _config;

        public JWToken(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJSONWebToken(User userInfo)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims
            var claimsList = new List<Claim>();
            claimsList.Add(new Claim("user_id", userInfo.Id.ToString()));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials,
                claims: claimsList
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
