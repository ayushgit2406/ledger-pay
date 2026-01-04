namespace Application.Interfaces;

public interface IOrderService
{
    Task<Guid> CreateOrderAsync(
        decimal amount,
        string currency,
        CancellationToken cancellationToken);
}