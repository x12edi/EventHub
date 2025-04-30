using EventHub.Core.Interfaces;
using EventHub.Infrastructure.Repositories;

namespace EventHub.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IEventRepository Events { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Events = new EventRepository(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}