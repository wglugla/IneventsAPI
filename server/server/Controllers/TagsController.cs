using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using LoggerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;

        public TagsController(ILoggerManager logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult> GetAllTags()
        {
            try
            {
                var tags = await _repository.Tag.GetAllTagsAsync();

                _logger.LogInfo($"Returned all tags from database.");

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllTags action: {ex.Message}");
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        // GET: api/Tags/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult> GetTagById(int id)
        {
            try
            {
                var tag = await _repository.Tag.GetTagByIdAsync(id);

                _logger.LogInfo($"Returned tag by id from database.");

                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTagById action: {ex.Message}");
                return StatusCode(500, "Internal server error" + ex);
            }
        }

        //// POST: api/Tags
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Tags/5
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
