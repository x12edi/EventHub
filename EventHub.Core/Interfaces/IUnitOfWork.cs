namespace EventHub.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        Task<int> SaveChangesAsync();
    }
}