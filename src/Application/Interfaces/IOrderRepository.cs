using Domain.Entities;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task UpdateAsync(Order order, CancellationToken cancellationToken);
    }
}