using System.Linq.Expressions;
namespace eBook_Library_Service.Models
{
    public interface IRepository <T> where T : class
    {
        Task<IEnumerable<T>>GetAllsync();
        Task<T>GetByIdAsync(int id,QueryOptions<T> options);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

            
    }
}
