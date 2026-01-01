namespace Domain.ValueObjects
{
    public sealed class IdempotencyKey
    {
        public string Value { get; }

        public IdempotencyKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Idempotency key cannot be empty.");
            }
            Value = value;
        }
    }
}