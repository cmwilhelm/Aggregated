using System;

namespace Aggregated.Model
{
    public sealed class FeedModel : IModel
    {
        public string Id { get; private set; }
        public Uri Uri { get; private set; }
        public string Name { get; private set; }

        public FeedModel(Uri uri, string name)
        {
            this.Uri = uri;
            this.Name = name;
        }

        public FeedModel(string id, Uri uri, string name)
            : this(uri, name)
        {
            this.Id = id;
        }
    }
}
