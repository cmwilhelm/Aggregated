using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Aggregated.IO.MongoDB.Extensions
{
    internal static class Extensions
    {
        public static IMongoQuery AsQuery(this IEnumerable<ObjectId> source)
        {
            return Query.In("_id", source.Select(i => (BsonValue)i));
        }
    }
}
