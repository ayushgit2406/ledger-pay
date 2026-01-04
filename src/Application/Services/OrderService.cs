using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IMessagePublisher _publisher;

    public OrderService(
        IOrderRepository orders,
        IMessagePublisher publisher)
    {
        _orders = orders;
        _publisher = publisher;
    }

    public async Task<Guid> CreateOrderAsync(
        decimal amount,
        string currency,
        CancellationToken cancellationToken)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.");

        var money = new Money(amount, currency);
        var order = new Order(money);

        await _orders.AddAsync(order, cancellationToken);

        await _publisher.PublishAsync(
    new
    {
        OrderId = order.Id
    },
    cancellationToken);

        return order.Id;
    }
}