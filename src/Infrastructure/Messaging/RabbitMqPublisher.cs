using System.Text;
using System.Text.Json;
using Application.Interfaces;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IConnection _connection;
        public RabbitMqPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public Task PublishAsync<T>(T message, CancellationToken ct)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare("payments", durable: true, exclusive: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish("", "payments", body: body);
            return Task.CompletedTask;
        }
    }
}