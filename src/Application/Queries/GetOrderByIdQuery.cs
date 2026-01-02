using Application.Interfaces;
using Domain.Entities;

namespace Application.Queries
{
    public sealed class GetOrderByIdQuery
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQuery(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<Order?> ExecuteAsync(Guid orderId, CancellationToken cancellationToken)
        {
            return _orderRepository.GetByIdAsync(orderId, cancellationToken);
        }
    }
}