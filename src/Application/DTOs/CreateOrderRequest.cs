namespace Application.DTOs
{
    public sealed class CreateOrderRequest
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; } = default!;
    }
}