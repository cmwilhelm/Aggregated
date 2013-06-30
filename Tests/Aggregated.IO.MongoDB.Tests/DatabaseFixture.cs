using System;
using MongoDB.Driver;

namespace Aggregated.IO.MongoDB.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public MongoDatabase Database { get; private set; }

        public DatabaseFixture()
        {
            this.Database = new MongoClient("mongodb://localhost")
                .GetServer()
                .GetDatabase(String.Format("aggregated_test_{0}", Guid.NewGuid().ToString("N")));
        }

        public void Dispose()
        {
            if (this.Database != null)
            {
                this.Database.Drop();
            }
        }
    }
}
