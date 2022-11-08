using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork<T> where T : class
    {
        IGenericRepository<T> Entity { get; }
        void Save();
        Task<int> SaveAsync();
    }
}
