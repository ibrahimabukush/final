

using eBook_Library_Service.Data;
using Microsoft.EntityFrameworkCore;

namespace eBook_Library_Service.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context { get; set; }
        private DbSet<T> _dbSet { get; set; }
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
         T entity = await _dbSet.FindAsync(id);
         _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllsync()
        {
           return await _dbSet.ToArrayAsync();
        }

        public async Task<T> GetByIdAsync(int id, QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;
            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }
            if (options.HasOrderBy)
            {
                query = query.OrderBy(options.OrderBy);
            }
            foreach (string include in options.GetIncludes())
            {
                query = query.Include(include);
            }   
            var key = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
            string primarykeyname = key.Name;
            return await query.FirstOrDefaultAsync(e=> EF.Property<int>(e, primarykeyname)==id);
        }

        public async Task UpdateAsync(T entity)
        {
         _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
