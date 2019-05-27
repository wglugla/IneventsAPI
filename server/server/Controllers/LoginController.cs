using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Contracts;
using LoggerServices;
using server.Helpers;

namespace server.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private IRepositoryWrapper _repository;
        private ILoggerManager _logger;

        public LoginController(IConfiguration config, IRepositoryWrapper repository, ILoggerManager logger)
        {
            _config = config;
            _repository = repository;
            _logger = logger;
        }

        // [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User userData)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(userData);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> AuthenticateUser(User login)
        {
            Encryption en = new Encryption();
            User user = null;
            string hashedPassword = login.Password;
            user = await _repository.User.GetUserByUsernameAsync(login.Username);
            if (en.Auth(hashedPassword, user.Password))
            {
                return user;
            }
            return null;
        }
    }
}
