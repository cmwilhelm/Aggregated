using NodaTime;

namespace Aggregated.Model
{
    public sealed class SnapshotModel : IModel
    {
        public string Id { get; private set; }
        public string FeedId { get; private set; }
        public Instant Retrieved { get; private set; }
        public string ContentType { get; private set; }
        public string Content { get; private set; }

        public SnapshotModel(string feedId, Instant retrieved, string contentType, string content)
        {
            this.FeedId = feedId;
            this.Retrieved = retrieved;
            this.ContentType = contentType;
            this.Content = content;
        }

        public SnapshotModel(string id, string feedId, Instant retrieved, string contentType, string content)
            : this(feedId, retrieved, contentType, content)
        {
            this.Id = id;
        }
    }
}
