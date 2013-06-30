using System.Collections.Generic;
using Aggregated.Model;

namespace Aggregated.IO
{
    public interface IRepository<T>
        where T : IModel
    {
        string Create(T model);
        IEnumerable<string> Create(IEnumerable<T> models);

        T Retrieve(string id);
        IEnumerable<T> Retrieve(IEnumerable<string> ids);

        void Update(T model);
        void Update(IEnumerable<T> models);

        void Delete(string id);
        void Delete(IEnumerable<string> ids);
    }
}
