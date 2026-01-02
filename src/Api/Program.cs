using Application.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LedgerPayDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var redisConnectionString =
    builder.Configuration.GetValue<string>("Redis:ConnectionString");

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
    throw new InvalidOperationException("RabbitMQ host is not configured.");
}

builder.Services.AddSingleton(_ =>
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

builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IIdempotencyStore, RedisIdempotencyStore>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
