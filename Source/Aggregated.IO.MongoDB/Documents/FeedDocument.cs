using MongoDB.Bson;

namespace Aggregated.IO.MongoDB.Documents
{
    public sealed class FeedDocument : IDocument
    {
        public ObjectId Id { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
    }
}
