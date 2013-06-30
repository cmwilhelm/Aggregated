using System;
using MongoDB.Bson;

namespace Aggregated.IO.MongoDB.Documents
{
    public sealed class SnapshotDocument : IDocument
    {
        public ObjectId Id { get; set; }
        public ObjectId FeedId { get; set; }
        public DateTime RetrievedUtc { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}
