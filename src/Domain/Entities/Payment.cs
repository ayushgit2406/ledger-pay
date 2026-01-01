using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Payment() { }

        public Payment(Guid orderId)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            Status = PaymentStatus.Initiated;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkSuccess()
        {
            if (Status != PaymentStatus.Initiated)
            {
                throw new InvalidOperationException("Only initiated payments can be success.");
            }
            Status = PaymentStatus.Success;
        }
        public void MarkFail()
        {
            if (Status != PaymentStatus.Initiated)
            {
                throw new InvalidOperationException("Only initiated payments can fail.");
            }
            Status = PaymentStatus.Failed;
        }
    }
}