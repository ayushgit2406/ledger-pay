using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Money Amount { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Order() { }

        public Order(Money amount)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be paid.");
            }
            Status = OrderStatus.Paid;
        }

        public void MarkAsFailed()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can fail.");
            }
            Status = OrderStatus.Failed;
        }
    }
}