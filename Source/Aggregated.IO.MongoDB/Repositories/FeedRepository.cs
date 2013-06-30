using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Aggregated.Common.DependencyInjection;
using Aggregated.Common.Extensions;
using Aggregated.IO.MongoDB.Documents;
using Aggregated.IO.MongoDB.Extensions;
using Aggregated.Model;
using MongoDB.Driver.Builders;

namespace Aggregated.IO.MongoDB.Repositories
{
    public sealed class FeedRepository : IRepository<FeedModel>
    {
        private readonly IFactory<MongoCollection<FeedDocument>> feedCollectionFactory;

        public FeedRepository(IFactory<MongoCollection<FeedDocument>> feedCollectionFactory)
        {
            this.feedCollectionFactory = feedCollectionFactory;
        }

        public string Create(FeedModel model)
        {
            return this.Create(model.AsEnumerable()).Single();
        }

        public IEnumerable<string> Create(IEnumerable<FeedModel> models)
        {
            var docs = models
                .Select(ConvertAsNew)
                .AsCollection();

            this.feedCollectionFactory.Make().InsertBatch(docs);

            return docs
                .Select(i => i.Id)
                .Select(i => i.ToString());
        }

        public FeedModel Retrieve(string id)
        {
            return this.Retrieve(id.AsEnumerable()).Single();
        }

        public IEnumerable<FeedModel> Retrieve(IEnumerable<string> ids)
        {
            return this.feedCollectionFactory.Make()
                .Find(ids.Select(ObjectId.Parse).AsQuery())
                .Select(Convert);
        }

        public IEnumerable<FeedModel> RetrieveAll()
        {
            return this.feedCollectionFactory.Make()
                .FindAll()
                .Select(Convert);
        }

        public void Update(FeedModel model)
        {
            this.Update(model.AsEnumerable());
        }

        public void Update(IEnumerable<FeedModel> models)
        {
            var feedCollection = this.feedCollectionFactory.Make();

            foreach (var feedDocument in models.Select(Convert))
            {
                var updateBuilder = new UpdateBuilder<FeedDocument>();
                updateBuilder.Set(i => i.Uri, feedDocument.Uri);
                updateBuilder.Set(i => i.Name, feedDocument.Name);

                feedCollection.Update(Query<FeedDocument>.EQ(i => i.Id, feedDocument.Id), updateBuilder);
            }
        }

        public void Delete(string id)
        {
            this.Delete(id.AsEnumerable());
        }

        public void Delete(IEnumerable<string> ids)
        {
            this.feedCollectionFactory.Make()
                .Remove(ids.Select(ObjectId.Parse).AsQuery());
        }

        #region Conversions

        private static FeedModel Convert(FeedDocument doc)
        {
            return doc.Id == ObjectId.Empty ?
                ConvertAsNew(doc) :
                new FeedModel(doc.Id.ToString(), new Uri(doc.Uri), doc.Name);
        }

        private static FeedModel ConvertAsNew(FeedDocument doc)
        {
            return new FeedModel(new Uri(doc.Uri), doc.Name);
        }

        private static FeedDocument Convert(FeedModel feed)
        {
            return feed.Id == null ?
                ConvertAsNew(feed) :
                new FeedDocument { Id = ObjectId.Parse(feed.Id), Uri = feed.Uri.ToString(), Name = feed.Name };
        }

        private static FeedDocument ConvertAsNew(FeedModel feed)
        {
            return new FeedDocument { Uri = feed.Uri.ToString(), Name = feed.Name };
        }

        #endregion
    }
}
