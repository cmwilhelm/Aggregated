using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NodaTime;
using Aggregated.Common.Extensions;
using Aggregated.IO.MongoDB.Extensions;
using Aggregated.Common.DependencyInjection;
using Aggregated.IO.MongoDB.Documents;
using Aggregated.Model;

namespace Aggregated.IO.MongoDB.Repositories
{
    public sealed class SnapshotRepository : IRepository<SnapshotModel>
    {
        private readonly MongoCollection<SnapshotDocument> collection;

        public SnapshotRepository(IFactory<MongoCollection<SnapshotDocument>> snapshotCollectionFactory)
        {
            this.collection = snapshotCollectionFactory.Make();
        }

        public string Create(SnapshotModel model)
        {
            return this.Create(model.AsEnumerable()).Single();
        }

        public IEnumerable<string> Create(IEnumerable<SnapshotModel> models)
        {
            var docs = models
                .Select(ConvertAsNew)
                .AsCollection();

            this.collection.InsertBatch(docs);

            return docs
                .Select(i => i.Id)
                .Select(i => i.ToString());
        }

        public SnapshotModel Retrieve(string id)
        {
            return this.Retrieve(id.AsEnumerable()).Single();
        }

        public IEnumerable<SnapshotModel> Retrieve(IEnumerable<string> ids)
        {
            return this.collection
                .Find(ids.Select(ObjectId.Parse).AsQuery())
                .Select(Convert);
        }

        public IEnumerable<SnapshotModel> RetrieveAll()
        {
            return this.collection
                .FindAll()
                .Select(Convert);
        }

        public void Update(SnapshotModel model)
        {
            this.Update(model.AsEnumerable());
        }

        public void Update(IEnumerable<SnapshotModel> models)
        {
            foreach (var snapshotDocument in models.Select(Convert))
            {
                var updateBuilder = new UpdateBuilder<SnapshotDocument>();
                updateBuilder.Set(i => i.FeedId, snapshotDocument.FeedId);
                updateBuilder.Set(i => i.RetrievedUtc, snapshotDocument.RetrievedUtc);
                updateBuilder.Set(i => i.ContentType, snapshotDocument.ContentType);
                updateBuilder.Set(i => i.Content, snapshotDocument.Content);

                this.collection.Update(Query<FeedDocument>.EQ(i => i.Id, snapshotDocument.Id), updateBuilder);
            }
        }

        public void Delete(string id)
        {
            this.Delete(id.AsEnumerable());
        }

        public void Delete(IEnumerable<string> ids)
        {
            this.collection
                .Remove(ids.Select(ObjectId.Parse).AsQuery());
        }

        #region Conversions

        private static SnapshotModel Convert(SnapshotDocument doc)
        {
            return doc.Id == ObjectId.Empty ?
                ConvertAsNew(doc) :
                new SnapshotModel(
                    doc.Id.ToString(),
                    Instant.FromDateTimeUtc(doc.RetrievedUtc),
                    doc.ContentType,
                    doc.Content
                );
        }

        private static SnapshotModel ConvertAsNew(SnapshotDocument doc)
        {
            return new SnapshotModel(
                doc.FeedId.ToString(),
                Instant.FromDateTimeUtc(doc.RetrievedUtc),
                doc.ContentType,
                doc.Content
            );
        }

        private static SnapshotDocument Convert(SnapshotModel model)
        {
            return model.Id == null ?
                ConvertAsNew(model) :
                new SnapshotDocument
                {
                    Id = ObjectId.Parse(model.Id),
                    FeedId = ObjectId.Parse(model.FeedId),
                    RetrievedUtc = model.Retrieved.ToDateTimeUtc(),
                    ContentType = model.ContentType,
                    Content = model.Content
                };
        }

        private static SnapshotDocument ConvertAsNew(SnapshotModel model)
        {
            return new SnapshotDocument
            {
                FeedId = ObjectId.Parse(model.FeedId),
                RetrievedUtc = model.Retrieved.ToDateTimeUtc(),
                ContentType = model.ContentType,
                Content = model.Content
            };
        }

        #endregion
    }
}
