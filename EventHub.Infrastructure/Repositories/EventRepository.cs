using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace EventHub.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<Event> GetBySlugAsync(string slug)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Slug == slug);
        }
        public async Task AddAsync(Event evt)
        {
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event evt)
        {
            _context.Events.Update(evt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt != null)
            {
                _context.Events.Remove(evt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> SearchAsync(string title, bool? isActive)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(e => e.Title.Contains(title));
            }

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            return await query.ToListAsync();
        }
    }
}