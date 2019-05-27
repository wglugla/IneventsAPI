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
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _repository.User.GetAllUsers();

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
        // TODO: schować hasło!!
        [HttpGet("{id}", Name = "UserById")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = _repository.User.GetUserById(id);

                if (user.Id.Equals(0))
                {
                    return NotFound();
                }
                else
                {
                    return Ok(user);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/users
        [HttpPost]
        public IActionResult CreateUser([FromBody]User user)
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
                Encryption en = new Encryption();
                user.Password = en.Encrypt(user.Password);
                _repository.User.CreateUser(user);
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
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _repository.User.GetUserById(id);
                if (user == null)
                {
                    _logger.LogError($"User with id {id}, hasn't been found in database");
                    return NotFound();
                }

                _repository.User.DeleteUser(user);
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
