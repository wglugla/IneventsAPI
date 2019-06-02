using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using LoggerServices;
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
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/users/5
        [HttpGet("{id}", Name = "UserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                User user = await _repository.User.GetUserByIdAsync(id);
                ProtectedUser response = new ProtectedUser();
                response.Id = user.Id;
                response.Username = user.Username;
                response.Name = user.Name;
                response.Surname = user.Surname;
                response.Create_time = user.Create_time;
                if (user.Id.Equals(0))
                {
                    return NotFound();
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
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
                user.Create_time = DateTime.Now;
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
