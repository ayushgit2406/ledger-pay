using Application.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentProcessor;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var dbConnectionString = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'Default' is not configured.");
}

builder.Services.AddDbContext<LedgerPayDbContext>(options =>
    options.UseNpgsql(dbConnectionString));

var redisConnectionString = builder.Configuration["Redis:ConnectionString"];

if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    throw new InvalidOperationException(
        "Redis connection string is not configured.");
}

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

var rabbitMqHost = builder.Configuration["RabbitMQ:Host"];

if (string.IsNullOrWhiteSpace(rabbitMqHost))
{
    throw new InvalidOperationException(
        "RabbitMQ host is not configured.");
}

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = rabbitMqHost,
        Port = builder.Configuration.GetValue<int>("RabbitMQ:Port"),
        UserName = builder.Configuration["RabbitMQ:Username"],
        Password = builder.Configuration["RabbitMQ:Password"]
    };

    return factory.CreateConnection();
});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IIdempotencyStore, RedisIdempotencyStore>();

builder.Services.AddHostedService<PaymentMessageConsumer>();

var app = builder.Build();
await app.RunAsync();