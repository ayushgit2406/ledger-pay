using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Commands
{
    public sealed class CreateOrderCommand
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessagePublisher _messagePublisher;

        public CreateOrderCommand(IOrderRepository orderRepository, IMessagePublisher messagePublisher)
        {
            _orderRepository = orderRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<CreateOrderResult> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var money = new Money(request.Amount, request.Currency);
            var order = new Order(money);

            await _orderRepository.AddAsync(order, cancellationToken);

            await _messagePublisher.PublishAsync(new { OrderId = order.Id }, cancellationToken);

            return new CreateOrderResult
            {
                OrderId = order.Id,
                Status = order.Status.ToString()
            };
        }
    }
}