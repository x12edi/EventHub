using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventHub.Application.Services;
using EventHub.Infrastructure.Repositories;

namespace EventHub.Web.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventRepository _repository;
        private readonly IEventService _service;

        public EventsController(IEventRepository repository, IEventService service)
        {
            _repository = repository;
            _service = service;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            if (ModelState.IsValid)
            {
                evt.Organizer = User.Identity.Name;
                await _repository.AddAsync(evt);
                return RedirectToAction(nameof(Index));
            }
            return View(evt);
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            var events = await _service.GetAllAsync();
            return View(events);
        }

        [HttpGet]
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
    }
}