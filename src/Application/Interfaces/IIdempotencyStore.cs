namespace Application.Interfaces
{
    public interface IIdempotencyStore
    {
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken);
        Task StoreAsync(string key, CancellationToken cancellationToken);
    }
}