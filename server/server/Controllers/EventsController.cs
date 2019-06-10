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
        /// <summary>
        /// Return all events in database
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /events
        ///     
        /// </remarks>
        /// <returns> Array of events </returns>
        /// <response code="200"> Return all events </response>
        /// <response code="400">If the item is null</response> 
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
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
                return NotFound();
                // return StatusCode(500, "Internal server error" + ex);
            }
        }

        /// <summary>
        /// Find event by tag id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /events/tag/1
        ///     
        /// </remarks>
        /// <returns> Array of events includes tag </returns>
        /// <response code="200"> Return event object </response>
        /// <response code="400"> If array is null </response> 
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
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

        /// <summary>
        /// Find event by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /events/1
        ///     
        /// </remarks>
        /// <returns> Event object </returns>
        /// <response code="200"> Return event object </response>
        /// <response code="400"> If the item is null </response> 
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
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

        /// <summary>
        /// Delete event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /events/1
        ///     
        /// </remarks>
        /// <response code="200"> Event successfully deleted </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="404"> Not found </response> 
        /// <response code="500"> Internal server error </response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent (int id)
        {
            try
            {
                Event target = await _repository.Event.GetEventByIdAsync(id);
                if (target == null)
                {
                    return NotFound();
                }
                await _repository.Event.DeleteEventAsync(target);
                _repository.Save();
                return Ok("Event successfully deleted");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /events/1
        ///     
        /// </remarks>
        /// <response code="200"> Event successfully modified </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="404"> Not found </response> 
        /// <response code="500"> Internal server error </response>
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyEvent (int id, [FromBody]Event ev)
        {
            try
            {
                Event target = await _repository.Event.GetEventByIdAsync(id);
                if (target == null)
                {
                    return NotFound();
                }
                target.Title = ev.Title;
                target.Description = ev.Description;
                target.Date = ev.Date;
                target.Place = ev.Place;
                await _repository.Event.ModifyEventAsync(target);
                return Ok("Event successfully modified");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Add user to event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /events/1/addmember
        ///     
        /// </remarks>
        /// <response code="200"> User successfully added </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="404"> Not found </response> 
        /// <response code="500"> Internal server error </response>
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

        /// <summary>
        /// Remove member from event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /events/1/removemember
        ///     
        /// </remarks>
        /// <response code="200"> User successfully removed </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="404"> Not found </response> 
        /// <response code="500"> Internal server error </response>
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

        /// <summary>
        /// Create new wevent
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /events
        ///     
        /// </remarks>
        /// <response code="201"> Event successfully created </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            try
            {
                await _repository.Event.CreateEventAsync(newEvent);
                return CreatedAtRoute("EventById", new { id = newEvent.Id }, newEvent);

            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e); 
            }
        }

        /// <summary>
        /// Add tags to event
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /events/1/tags
        ///     
        /// </remarks>
        /// <response code="200"> Tags successfully added </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="500"> Internal server error </response>
        [HttpPut("{id}/tags")]
        public async Task<IActionResult> ChangeEventTags(int id, [FromBody]int[] tagIds)
        {
            try
            {
                EventsTags[] oldTags = await _repository.EventsTags.FindByCondition(p => p.EventId.Equals(id)).ToArrayAsync();
                foreach (EventsTags tag in oldTags)
                {
                    _repository.EventsTags.Delete(tag);
                }
                await _repository.EventsTags.SaveAsync();

                foreach (int i in tagIds)
                {
                    EventsTags newTag = new EventsTags()
                    {
                        EventId = id,
                        TagId = i
                    };
                    _repository.EventsTags.Create(newTag);
                }
                await _repository.EventsTags.SaveAsync();
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
