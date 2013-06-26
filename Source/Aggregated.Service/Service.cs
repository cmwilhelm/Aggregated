using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using MongoDB.Driver;
using NodaTime;
using Aggregated.Common.DependencyInjection;
using Aggregated.IO;
using Aggregated.IO.MongoDB.Documents;
using Aggregated.IO.MongoDB.Repositories;
using Aggregated.Model;

namespace Aggregated.Service
{
    internal class Service : ServiceBase
    {
        private class MongoCollectionFactory :
            IFactory<MongoCollection<FeedDocument>>,
            IFactory<MongoCollection<SnapshotDocument>>
        {
            private readonly MongoDatabase database;

            public MongoCollectionFactory()
            {
                this.database = new MongoClient("mongodb://localhost").GetServer().GetDatabase("aggregated");
            }

            MongoCollection<FeedDocument> IFactory<MongoCollection<FeedDocument>>.Make()
            {
                return this.database.GetCollection<FeedDocument>("feeds");
            }

            MongoCollection<SnapshotDocument> IFactory<MongoCollection<SnapshotDocument>>.Make()
            {
                return this.database.GetCollection<SnapshotDocument>("snapshots");
            }
        }

        private static readonly ManualResetEventSlim WaitHandle = new ManualResetEventSlim();
        private static Timer timer;

        private IRepository<FeedModel> feedRepository;
        private IRepository<SnapshotModel> snapshotRepository;

        protected override void OnStart(string[] args)
        {
            var collectionFactory = new MongoCollectionFactory();

            this.feedRepository = new FeedRepository(collectionFactory);
            this.snapshotRepository = new SnapshotRepository(collectionFactory);

            timer = new Timer(DownloadFeeds);
            timer.Change(0, 60000);
        }

        private void DownloadFeeds(object state)
        {
            var snapshots = new List<SnapshotModel>();

            foreach (var feed in this.feedRepository.RetrieveAll())
            {
                try
                {
                    Console.WriteLine(@"Downloading: ""{0}"" @ {1}", feed.Name, feed.Uri);

                    if (feed.Uri.Scheme == "http" || feed.Uri.Scheme == "https")
                    {
                        var webClient = new WebClient();

                        var content = webClient.DownloadString(feed.Uri);
                        var contentType = webClient.ResponseHeaders.GetValues("Content-Type").FirstOrDefault();
                        var retrieved = SystemClock.Instance.Now;

                        snapshots.Add(new SnapshotModel(feed.Id, retrieved, contentType, content));
                    }
                    else
                    {
                        Console.Error.WriteLine("Failed: Do not how to retrieve feed with scheme of '{0}'", feed.Uri.Scheme);
                    }
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Failed");
                }
            }

            this.snapshotRepository.Create(snapshots);
        }

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) => WaitHandle.Set();

            var service = new Service();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);

                WaitHandle.Wait();

                service.OnStop();
            }
            else
            {
                Run(service);
            }
        }
    }
}
