using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventHub.Application.Services;
using EventHub.Infrastructure.Repositories;
using EventHub.Web.Filters;

namespace EventHub.Web.Controllers
{
    [Authorize]
    [Route("Events")]
    [ServiceFilter(typeof(ActionLogFilter))]
    public class EventsController : Controller
    {
        private readonly IEventService _service;

        public EventsController(IEventService service)
        {
            _service = service;
        }
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            if (ModelState.IsValid)
            {
                evt.Organizer = User.Identity.Name;
                await _service.AddAsync(evt);
                return RedirectToAction(nameof(Index));
            }
            return View(evt);
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var events = await _service.GetAllAsync();
            return View(events);
        }

        [HttpGet]
        [Route("Index2")]
        public async Task<IActionResult> Index2()
        {
            var events = await _service.GetAllAsync();
            return View(events);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt == null)
            {
                return NotFound();
            }
            return View(evt);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            if (id != evt.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingEvent = await _service.GetByIdAsync(id);
                if (existingEvent == null)
                {
                    return NotFound();
                }

                // Update fields
                existingEvent.Title = evt.Title;
                existingEvent.Description = evt.Description;
                existingEvent.StartDate = evt.StartDate;
                existingEvent.IsActive = evt.IsActive;
                existingEvent.Organizer = User.Identity.Name; // Preserve or update organizer

                await _service.UpdateAsync(existingEvent);
                return RedirectToAction(nameof(Index));
            }
            return View(evt);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt == null)
            {
                return NotFound();
            }
            return View(evt);
        }

        [HttpPost, ActionName("Delete")]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("Details/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var evt = await _service.GetBySlugAsync(slug);
            if (evt == null)
            {
                return NotFound();
            }
            return View(evt);
        }

        [HttpGet]
        [Route("GetEventDetails/{id}")]
        public async Task<IActionResult> GetEventDetails(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt == null)
            {
                return NotFound();
            }
            return Json(new
            {
                title = evt.Title,
                description = evt.Description,
                startDate = evt.StartDate.ToString("d"),
                organizer = evt.Organizer,
                isActive = evt.IsActive ? "Active" : "Inactive"
            });
        }
    }
}