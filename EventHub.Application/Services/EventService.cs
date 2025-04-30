using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Infrastructure.Repositories;

namespace EventHub.Application.Services
{
    public interface IEventService
    {
        Task<List<Event>> GetAllAsync();
        Task<Event> GetByIdAsync(int id);
        Task AddAsync(Event evt);
        Task UpdateAsync(Event evt);
        Task DeleteAsync(int id);

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

        public async Task AddAsync(Event evt)
        {
            await _unitOfWork.Events.AddAsync(evt);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event evt)
        {
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
    }
}