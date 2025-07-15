using System.Linq.Expressions;

namespace Do_an_II.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Vi du T = Product,Catagory,User,...
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter,bool tracked = false, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
