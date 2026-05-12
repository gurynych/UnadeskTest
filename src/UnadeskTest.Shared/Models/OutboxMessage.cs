namespace UnadeskTest.Shared.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Pending;

        public int PublishAttempts { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? PublishedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
