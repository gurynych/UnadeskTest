namespace UnadeskTest.Shared.Models
{
    public enum OutboxMessageStatus
    {
        Pending = 1,
        Published = 2,
        Failed = 3
    }
}
