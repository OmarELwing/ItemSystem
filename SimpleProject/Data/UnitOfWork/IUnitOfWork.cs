using SimpleProject.Data.Models;
using SimpleProject.Data.Repos;
using SimpleProject.Models;

namespace SimpleProject.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepo<Category> Categories { get; }
        IRepo<Item> Items { get; }
        Task SaveAsync();
    }
}
