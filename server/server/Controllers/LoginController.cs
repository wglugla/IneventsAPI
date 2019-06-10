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
using Authorization;

namespace server.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private IRepositoryWrapper _repository;
        private ILoggerManager _logger;
        private JWToken JWT;
        private Encryption encryptor;

        public LoginController(IConfiguration config, IRepositoryWrapper repository, ILoggerManager logger)
        {
            _config = config;
            _repository = repository;
            _logger = logger;
            JWT = new JWToken(config);
            encryptor = new Encryption(repository);
        }

        /// <summary>
        /// Login to system
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /login
        ///     
        /// </remarks>
        /// <response code="200"> Success. Returns login and JSON web token </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User userData)
        {
            IActionResult response = Unauthorized();
            try
            {
                var user = await encryptor.AuthenticateUser(userData);
                if (user != null)
                {
                    var tokenString = JWT.GenerateJSONWebToken(user);
                    response = Ok(new {id = user.Id, token = tokenString });
                }
                else
                {
                    response = Unauthorized();
                }
                return response;
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e);
            }
        }
    }
}
