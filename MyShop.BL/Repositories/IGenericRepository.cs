using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        //_context.Categories.include("Products").ToList();
        //_context.Categories.where(x=> x.Id == id).ToList();
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? predicate= null, string? includeword= null);

        //_context.Categories.include("Products").ToSingleOrDefault();
        //_context.Categories.where(x=> x.Id == id).ToSingleOrDefault();

        T GetFirstOrDefault(Expression<Func<T, bool>>  predicate, string? includeword = null);

        //_context.Categories.Add(category);
        void Add(T entity);

        //_context.Categories.Remove(category);
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

    }
}
