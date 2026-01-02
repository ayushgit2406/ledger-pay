namespace Application.Interfaces
{
    public interface IPaymentProvider
    {
        Task<bool> ProcessAsync(Guid orderId, CancellationToken cancellationToken);
    }
}