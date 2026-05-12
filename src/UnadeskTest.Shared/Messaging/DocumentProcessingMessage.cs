namespace UnadeskTest.Shared.Messaging
{
    public class DocumentProcessingMessage
    {
        public Guid MessageId { get; set; }

        public Guid DocumentId { get; set; }
    }
}
