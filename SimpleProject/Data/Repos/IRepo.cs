using System.Linq.Expressions;

namespace SimpleProject.Data.Repos;

public interface IRepo<T>
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}