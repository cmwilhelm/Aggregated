using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Moq;
using Xunit;
using Aggregated.Common.DependencyInjection;
using Aggregated.Common.Extensions;
using Aggregated.IO.MongoDB.Documents;
using Aggregated.IO.MongoDB.Repositories;
using Aggregated.Model;

namespace Aggregated.IO.MongoDB.Tests.Repositories
{
    public sealed class FeedRepositoryTests : IUseFixture<DatabaseFixture>
    {
        private MongoCollection<FeedDocument> feedDocumentCollection;

        private FeedRepository systemUnderTest;

        #region IUseFixture

        public void SetFixture(DatabaseFixture data)
        {
            this.feedDocumentCollection = data.Database.GetCollection<FeedDocument>("feeds");

            var feedCollectionFactory = new Mock<IFactory<MongoCollection<FeedDocument>>>(MockBehavior.Strict);
            feedCollectionFactory.Setup(i => i.Make())
                .Returns(this.feedDocumentCollection);

            this.systemUnderTest = new FeedRepository(feedCollectionFactory.Object);
        }

        #endregion

        #region Facts

        [Fact]
        public void CreateCanInsertSingleFeed()
        {
            // Arrange
            var testFeedModel = MakeTestFeedModel();

            // Act
            var id = this.systemUnderTest.Create(testFeedModel);

            // Assert
            var feedDocument = this.GetById(id);

            feedDocument.Uri.Should().Be(testFeedModel.Uri.ToString(),
                "Uri should be stored exactly as provided"
            );
            feedDocument.Name.Should().Be(testFeedModel.Name,
                "Name should be stored exactly as provided"
            );
        }

        [Fact]
        public void CreateCanInsertMultipleFeeds()
        {
            // Arrange
            const int testFeedCount = 10;

            var testFeedModels = MakeTestFeedModels(testFeedCount).AsCollection();

            // Act
            var ids = this.systemUnderTest.Create(testFeedModels);

            // Assert
            var feedDocuments = this.GetById(ids).AsCollection();

            feedDocuments.Should().HaveCount(testFeedCount,
                "Create() should store as many feeds as given"
            );

            foreach (var testFeedModel in testFeedModels)
            {
                var tfm = testFeedModel;

                feedDocuments.Should().Contain(i => i.Uri == tfm.Uri.ToString(),
                    "Uri should be stored exactly as provided"
                );

                feedDocuments.Should().Contain(i => i.Name == tfm.Name,
                    "Name should be stored exactly as provided"
                );
            }
        }

        [Fact]
        public void CreateGeneratesNewIdIfFeedAlreadyHasOne()
        {
            var testFeedModel = MakeTestFeedModel(generateId: true);

            var id = this.systemUnderTest.Create(testFeedModel);

            id.Should().NotBe(testFeedModel.Id,
                "Create() should always insert a new document, not update an existing one"
            );
        }

        [Fact]
        public void RetrieveCanGetSingleFeed()
        {
            // Arrange
            var testFeedDocument = MakeTestFeedDocument();
            this.feedDocumentCollection.Insert(testFeedDocument);

            // Act
            var feedModel = this.systemUnderTest.Retrieve(testFeedDocument.Id.ToString());

            // Assert
            feedModel.Id.Should().Be(testFeedDocument.Id.ToString(),
                "Retrieve() should get the correct feed"
            );

            feedModel.Uri.Should().Be(new Uri(testFeedDocument.Uri),
                "Retrieve() should get the correct Uri"
            );

            feedModel.Name.Should().Be(testFeedDocument.Name,
                "Retrieve() should get the correct Name"
            );
        }

        [Fact]
        public void RetrieveCanGetMultipleFeeds()
        {
            // Arrange
            const int testFeedDocumentCount = 10;

            var testFeedDocuments = MakeTestFeedDocuments(testFeedDocumentCount).AsCollection();
            this.feedDocumentCollection.InsertBatch(testFeedDocuments);

            // Act
            var feedModels = this.systemUnderTest
                .Retrieve(testFeedDocuments.Select(i => i.Id).Select(i => i.ToString()))
                .AsCollection();

            // Assert
            feedModels.Should().HaveCount(testFeedDocumentCount);

            foreach (var testFeedDocument in testFeedDocuments)
            {
                var tfd = testFeedDocument;

                feedModels.Should().Contain(i => i.Uri == new Uri(tfd.Uri),
                    "Retrieve() should get Uri exactly as provided"
                );

                feedModels.Should().Contain(i => i.Name == tfd.Name,
                    "Retrieve() should get Name exactly as provided"
                );
            }
        }

        [Fact]
        public void UpdateCanReplaceSingleFeed()
        {
            // Arrange
            var testFeedDocument = MakeTestFeedDocument();
            this.feedDocumentCollection.Insert(testFeedDocument);

            var updateFeedModel = new FeedModel(
                testFeedDocument.Id.ToString(),
                new Uri(testFeedDocument.Uri + "updated"),
                testFeedDocument.Name + " - Updated"
            );

            // Act
            this.systemUnderTest.Update(updateFeedModel);

            // Assert
            var updatedFeedDocument = this.GetById(testFeedDocument.Id.ToString());

            updatedFeedDocument.Uri.Should().Be(updateFeedModel.Uri.ToString(),
                "Update() should replace Uri exactly as provided"
            );

            updatedFeedDocument.Name.Should().Be(updateFeedModel.Name,
                "Update() should replace Name exactly as provided"
            );
        }

        [Fact]
        public void UpdateCanReplaceMultipleFeeds()
        {
            // Arrange
            var testFeedDocuments = MakeTestFeedDocuments(10).AsCollection();
            this.feedDocumentCollection.InsertBatch(testFeedDocuments);

            var updateFeedModels = testFeedDocuments.Select(i =>
                new FeedModel(
                    i.Id.ToString(),
                    new Uri(i.Uri + "/updated"),
                    i.Name + " - Updated"
                )
            ).AsCollection();

            // Act
            this.systemUnderTest.Update(updateFeedModels);

            // Assert
            var updatedFeedDocuments = this.GetById(testFeedDocuments.Select(i => i.Id).Select(i => i.ToString()))
                .AsCollection();

            foreach (var updateFeedModel in updateFeedModels)
            {
                var ufm = updateFeedModel;

                updatedFeedDocuments.Should().Contain(i => i.Uri == ufm.Uri.ToString(),
                    "Update() should replace Uri exactly as provided"
                );

                updatedFeedDocuments.Should().Contain(i => i.Name == ufm.Name.ToString(),
                    "Update() should replace Name exactly as provided"
                );
            }
        }

        [Fact]
        public void DeleteCanRemoveSingleFeed()
        {
            // Arrange
            var testFeedDocument = MakeTestFeedDocument();
            this.feedDocumentCollection.Insert(testFeedDocument);

            var id = testFeedDocument.Id.ToString();

            // Act
            this.systemUnderTest.Delete(id);

            // Assert
            var feedDocument = this.GetById(id);

            feedDocument.Should().BeNull("Delete() should remove document from database");
        }

        [Fact]
        public void DeleteCanRemoveMultipleFeeds()
        {
            // Arrange
            var testFeedDocuments = MakeTestFeedDocuments(10).AsCollection();
            this.feedDocumentCollection.InsertBatch(testFeedDocuments);

            var ids = testFeedDocuments.Select(i => i.Id).Select(i => i.ToString()).AsCollection();

            // Act
            this.systemUnderTest.Delete(ids);

            // Assert
            var feedDocuments = this.GetById(ids);

            feedDocuments.Should().BeEmpty("Delete() should remove documents from database");
        }

        #endregion

        #region Test Helpers

        private FeedDocument GetById(string id)
        {
            return this.GetById(id.AsEnumerable()).SingleOrDefault();
        }

        private IEnumerable<FeedDocument> GetById(IEnumerable<string> ids)
        {
            return this.feedDocumentCollection.Find(
                Query<FeedDocument>.In(i => i.Id, ids.Select(ObjectId.Parse))
            );
        }

        private static FeedModel MakeTestFeedModel(bool generateId = false)
        {
            return MakeTestFeedModels(1, generateId).Single();
        }

        private static IEnumerable<FeedModel> MakeTestFeedModels(int count, bool generateIds = false)
        {
            const string baseUri = "http://feed.example.com/";
            const string baseName = "Test Feed";

            return Enumerable.Range(1, count).Select(i =>
                new FeedModel(
                    generateIds ? ObjectId.GenerateNewId().ToString() : ObjectId.Empty.ToString(),
                    new Uri(String.Format("{0}{1}", baseUri, i)),
                    String.Format("{0} #{1}", baseName, i)
                )
            );
        }

        private static FeedDocument MakeTestFeedDocument()
        {
            return MakeTestFeedDocuments(1).Single();
        }

        private static IEnumerable<FeedDocument> MakeTestFeedDocuments(int count)
        {
            const string baseUri = "http://feed.example.com/";
            const string baseName = "Test Feed";

            return Enumerable.Range(1, count).Select(i =>
                new FeedDocument
                {
                    Uri = String.Format("{0}{1}", baseUri, i),
                    Name = String.Format("{0} #{1}", baseName, i),
                }
            );
        }

        #endregion
    }
}
