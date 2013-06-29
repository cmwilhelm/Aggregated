using MongoDB.Bson;

namespace Aggregated.IO.MongoDB
{
    public interface IDocument
    {
        ObjectId Id { get; set; }
    }
}
