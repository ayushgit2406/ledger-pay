namespace Application.Interfaces
{
    public interface IAuditService
    {
        Task WriteAsync(string eventType, string payload, CancellationToken cancellationToken);
    }
}