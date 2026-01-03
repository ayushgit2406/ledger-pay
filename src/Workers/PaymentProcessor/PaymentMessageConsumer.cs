using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentProcessor;

public sealed class PaymentMessageConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;

    public PaymentMessageConsumer(
        IServiceScopeFactory scopeFactory,
        IConnection connection)
    {
        _scopeFactory = scopeFactory;
        _connection = connection;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(
            queue: "payments",
            durable: true,
            exclusive: false);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (_, args) =>
        {
            using var scope = _scopeFactory.CreateScope();

            var idempotency = scope.ServiceProvider
                .GetRequiredService<IIdempotencyStore>();
            var orders = scope.ServiceProvider
                .GetRequiredService<IOrderRepository>();
            var payments = scope.ServiceProvider
                .GetRequiredService<IPaymentRepository>();

            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            var message = JsonSerializer.Deserialize<PaymentMessage>(body)!;

            var key = $"payment:{message.OrderId}";

            if (await idempotency.ExistsAsync(key, stoppingToken))
            {
                channel.BasicAck(args.DeliveryTag, false);
                return;
            }

            var order = await orders.GetByIdAsync(
                message.OrderId, stoppingToken);

            if (order is null)
            {
                channel.BasicAck(args.DeliveryTag, false);
                return;
            }

            // Simulate payment success
            var payment = new Payment(order.Id);
            payment.MarkSuccess();
            order.MarkAsPaid();

            await payments.AddAsync(payment, stoppingToken);
            await orders.UpdateAsync(order, stoppingToken);
            await idempotency.StoreAsync(key, stoppingToken);

            channel.BasicAck(args.DeliveryTag, false);
        };

        channel.BasicConsume("payments", autoAck: false, consumer);

        return Task.CompletedTask;
    }
}