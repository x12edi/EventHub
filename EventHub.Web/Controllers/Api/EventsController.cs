using EventHub.Application.Services;
using EventHub.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EventHub.Web.Controllers.Api
{
    //[Route("api/events")]
    [Route("api/v{version:apiVersion}/events")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsV1()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<EventSummaryDto>>> GetEventsV2()
        {
            var events = await _eventService.GetAllAsync();
            var summaries = events.Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Title = e.Title,
                StartDate = e.StartDate,
                IsActive = e.IsActive
            });
            return Ok(summaries);
        }


        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var evt = await _eventService.GetByIdAsync(id);
            if (evt == null)
            {
                return NotFound();
            }
            return Ok(evt);
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event evt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            evt.Organizer = User.Identity.Name;
            await _eventService.AddAsync(evt);
            return CreatedAtAction(nameof(GetEvent), new { id = evt.Id }, evt);
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event evt)
        {
            if (id != evt.Id)
            {
                return BadRequest();
            }

            try
            {
                evt.Organizer = User.Identity.Name;
                await _eventService.UpdateAsync(evt);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }

    public class EventSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}