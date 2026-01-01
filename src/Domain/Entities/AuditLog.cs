namespace Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public string EventType { get; private set; }
        public string Payload { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private AuditLog() { }

        public AuditLog(string eventType, string payload)
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            Payload = payload;
            CreatedAt = DateTime.UtcNow;
        }
    }
}