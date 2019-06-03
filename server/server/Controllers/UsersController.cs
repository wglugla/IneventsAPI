using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using LoggerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Helpers;

namespace server.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;

        public UsersController(ILoggerManager logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _repository.User.GetAllUsersAsync();

                _logger.LogInfo($"Returned all owners from database.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        // GET api/users/5
        // Authentication: bearer token!
        [HttpGet("{id}", Name = "UserById")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                User user = await _repository.User.GetUserByIdAsync(id);
                _logger.LogInfo(user.Id.ToString());
                if (user.Id.Equals(0))
                {
                    return NotFound("test");
                }
                else
                {
                    object result = new
                    {
                        user.Id,
                        user.Username,
                        user.Name,
                        user.Surname,
                    };
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError("User object sent from client is null");
                    return BadRequest("User object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid user object sent from client");
                    return BadRequest("Invalid model object");
                }
                Encryption en = new Encryption(_repository);
                user.Password = en.Encrypt(user.Password);
                await _repository.User.CreateUserAsync(user);
                _repository.Save();
                
                // return 201 status
                return CreatedAtRoute("UserById", new { id = user.Id }, user);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/users
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _repository.User.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogError($"User with id {id}, hasn't been found in database");
                    return NotFound();
                }

                await _repository.User.DeleteUserAsync(user);
                _repository.Save();

                _logger.LogInfo($"Used with id {id} deleted");
                return Ok("User removed successfully");
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


    }  
}
