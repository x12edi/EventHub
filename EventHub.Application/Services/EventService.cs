using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Infrastructure.Repositories;

namespace EventHub.Application.Services
{
    public interface IEventService
    {
        Task<List<Event>> GetAllAsync();
        Task<Event> GetByIdAsync(int id);
        Task<Event> GetBySlugAsync(string slug);
        Task AddAsync(Event evt);
        Task UpdateAsync(Event evt);
        Task DeleteAsync(int id);
        Task<IEnumerable<Event>> SearchAsync(string title, bool? isActive);

    }

    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _unitOfWork.Events.GetAllAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _unitOfWork.Events.GetByIdAsync(id);
        }

        public async Task<Event> GetBySlugAsync(string slug)
        {
            return await _unitOfWork.Events.GetBySlugAsync(slug);
        }
        public async Task AddAsync(Event evt)
        {
            evt.Slug = GenerateSlug(evt.Title, evt.StartDate);
            await _unitOfWork.Events.AddAsync(evt);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event evt)
        {
            evt.Slug = GenerateSlug(evt.Title, evt.StartDate);
            await _unitOfWork.Events.UpdateAsync(evt);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var evt = await _unitOfWork.Events.GetByIdAsync(id);
            if (evt == null)
            {
                throw new InvalidOperationException("Event not found.");
            }
            await _unitOfWork.Events.DeleteAsync(id);
        }

        private string GenerateSlug(string title, DateTime startDate)
        {
            var slug = title.ToLower().Replace(" ", "-").Replace(".", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, "[^a-z0-9-]", "");
            return $"{slug}-{startDate.Year}";
        }

        public async Task<IEnumerable<Event>> SearchAsync(string title, bool? isActive)
        {
            return await _unitOfWork.Events.SearchAsync(title, isActive);
        }
    }
}