
using Microsoft.EntityFrameworkCore;
using MvcAndApixUnitTest.Web.Models;

namespace MvcAndApixUnitTest.Web.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly XUnitTestDbContext _context;
        private readonly DbSet<TEntity> _entities;

        public Repository(XUnitTestDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public async Task Create(TEntity entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
