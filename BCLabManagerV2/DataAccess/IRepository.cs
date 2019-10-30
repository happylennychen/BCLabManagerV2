using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.DataAccess
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string includeProperties = "");
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T GetById(object Id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
        void Delete(T obj);
    }
}
