using Microsoft.EntityFrameworkCore;
using SimpleProject.Data;
using System.Linq.Expressions;

namespace SimpleProject.Data.Repos;

public class Repo<T> : IRepo<T> where T : class
{
    private readonly AppDbContext _db;

    public Repo(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _db.Set<T>().ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _db.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _db.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _db.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
    }

    public async Task AddAsync(T entity)
    {
        await _db.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        _db.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _db.Set<T>().Remove(entity);
    }
}