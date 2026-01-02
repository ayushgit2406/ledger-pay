using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken cancellationToken);
    }
}