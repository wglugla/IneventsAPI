using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using LoggerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Get all users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users
        ///     
        /// </remarks>
        /// <response code="200"> Array of users </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
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

        /// <summary>
        /// Get events created by user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/1/created
        ///     
        /// </remarks>
        /// <response code="200"> Array of events created by user </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpGet("{id}/created")]
        public async Task<IActionResult> GetEventsByUser(int id)
        {
            try
            {
                Event[] events = await _repository.Event.FindByCondition(p => p.OwnerId.Equals(id)).ToArrayAsync();
                return Ok(events);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/1
        ///     
        /// </remarks>
        /// <response code="200"> User object </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpGet("{id}", Name = "UserById")]
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

        /// <summary>
        /// Get events in which user takes part in
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/1/signed
        ///     
        /// </remarks>
        /// <response code="200"> Array of events </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpGet("{id}/signed")]
        public async Task<IActionResult> GetUserSignedEvents(int id)
        {
            try
            {
                int[] signedEventsid = await _repository.UsersEvents.GetUsersEventsAsync(id);
                List<Event> events = new List<Event>();
                foreach (int i in signedEventsid)
                {
                    Event e = await _repository.Event.GetEventByIdAsync(i);
                    events.Add(e);
                }
                return Ok(events);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get events ids in which user takes part in
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/1/signedId
        ///     
        /// </remarks>
        /// <response code="200"> Array of events ids </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpGet("{id}/signedId")]
        public async Task<IActionResult> GetUserSignedEventsId(int id)
        {
            try
            {
                int[] signedEventsId = await _repository.UsersEvents.GetUsersEventsAsync(id);
                return Ok(signedEventsId);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get user tags
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/1/tags
        ///     
        /// </remarks>
        /// <response code="200"> Array of tags </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpGet("{id}/tags")]
        public async Task<IActionResult> GetUserTags(int id)
        {
            try
            {
                int[] userTagsId = await _repository.UsersTags.GetUserTagsAsync(id);
                Tag[] userTags = await _repository.Tag.FindByCondition(p => userTagsId.Contains(p.Id)).ToArrayAsync();
                return Ok(userTags);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Update user tags
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /users/1/tags
        ///     
        /// </remarks>
        /// <response code="200"> Tags successfully updated </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpPut("{id}/tags")]
        public async Task<IActionResult> ChangeUserTags(int id, [FromBody]int[] tagIds)
        {
            try
            {
                UsersTags[] oldTags = await _repository.UsersTags.FindByCondition(p => p.UserId.Equals(id)).ToArrayAsync();
                foreach(UsersTags tag in oldTags)
                {
                    _repository.UsersTags.Delete(tag);
                }
                await _repository.UsersTags.SaveAsync();

                foreach(int i in tagIds)
                {
                    UsersTags newTag = new UsersTags()
                    {
                        UserId = id,
                        TagId = i
                    };
                    _repository.UsersTags.Create(newTag);
                }
                await _repository.UsersTags.SaveAsync();
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /users
        ///     
        /// </remarks>
        /// <response code="201"> User successfully required </response>
        /// <response code="400"> Bad request </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
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

        /// <summary>
        /// Delete user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /users/1
        ///     
        /// </remarks>
        /// <response code="201"> User successfully required </response>
        /// <response code="404"> Not found </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
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
