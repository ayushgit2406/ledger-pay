namespace Application.DTOs
{
    public sealed class CreateOrderResult
    {
        public Guid OrderId { get; init; }
        public string Status { get; init; } = default!;
    }
}