using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostHost.Controllers
{
    public interface IDataAccessLayer <T>
    {
        void Add(T obj);

        void Delete(int ID);

        void Edit(int ID);

        T Read(int ID);

        List<T> GetFrom(IEnumerable<T> que);
    }
}