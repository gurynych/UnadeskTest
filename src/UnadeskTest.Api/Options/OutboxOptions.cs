namespace UnadeskTest.Api.Options
{
    public class OutboxOptions
    {
        public const string SectionName = "Outbox";

        public int BatchSize { get; set; } = 20;

        public int PublishIntervalSeconds { get; set; } = 2;

        public int MaxPublishAttempts { get; set; } = 5;
    }
}
