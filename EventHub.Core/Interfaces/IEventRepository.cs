using EventHub.Core.Entities;

namespace EventHub.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync();
        Task<Event> GetByIdAsync(int id);
        Task<Event> GetBySlugAsync(string slug);
        Task AddAsync(Event evt);
        Task UpdateAsync(Event evt);
        Task DeleteAsync(int id);
    }
}