using SimpleProject.Data.Models;
using SimpleProject.Data.Repos;
using SimpleProject.Models;

namespace SimpleProject.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public IRepo<Category> Categories { get; }
        public IRepo<Item> Items { get; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Categories = new Repo<Category>(_db);
            Items = new Repo<Item>(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
