using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using LoggerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Helpers;

namespace server.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;

        public EventsController(ILoggerManager logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _repository.Event.GetAllEventsAsync();
                _logger.LogInfo($"Returned all owners from database.");

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        // GET: api/events/tag/1
        [HttpGet("tag/{id}", Name ="EventsByTag")]
        public async Task<IActionResult> GetEventsByTag(int id)
        {
            try
            {
                int[] eventsId = await _repository.EventsTags.GetEventsByTag(id);
                Event[] events = await _repository.Event.FindByCondition(p => eventsId.Contains(p.Id)).ToArrayAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside EventsByTag action: {ex.Message}");
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        // GET: api/Events/5
        [HttpGet("{id}", Name =  "EventById")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                Event res = await _repository.Event.GetEventByIdAsync(id);
                int[] tagsId = await _repository.EventsTags.GetEventTagsAsync(res.Id);
                List<Tag> tags = new List<Tag>();
                foreach(int tagId in tagsId)
                {
                    Tag newTag = await _repository.Tag.GetTagByIdAsync(tagId);
                    tags.Add(newTag);
                }
                EventDetails details = new EventDetails(res, tags.Select(p => p.Value).ToArray());
                _logger.LogInfo($"Returned event by id from database.");

                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventById action: {ex.Message}");
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        // POST: api/events/addmember
        [HttpPost("{eventId}/addmember")]
        public async Task<IActionResult> AddMember(int eventId, [FromBody]UsersEvents userId)
        {
            try
            {
                int[] obj = await _repository.UsersEvents.GetUsersEventsAsync(userId.UserId);
                for (int i=0; i<obj.Length; i++)
                {
                    if (obj[i] == eventId)
                    {
                        return BadRequest("User is already a member!");
                    }
                }
                await _repository.UsersEvents.AddMemberAsync(userId.UserId, eventId);
                _repository.Save();

                // return 201 status
                return CreatedAtRoute("UserById", userId, userId);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside AddMember action: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/events/removemember
        [HttpDelete("{eventId}/removemember/{userId}")]
        public async Task<IActionResult> RemoveMember(int eventId, int userId)
        {
            try
            {
                await _repository.UsersEvents.RemoveMemberAsync(userId, eventId);
                _repository.Save();

                return Ok("User successfully removed");
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside RemoveMember action: {e.Message}");
                return StatusCode(500, "Internal server error" + e);
            }
        }

        // POST: api/Events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            try
            {
                return CreatedAtRoute("EventById", new { id = newEvent.Id }, newEvent);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e); 
            }
        }

        //// PUT: api/Events/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
