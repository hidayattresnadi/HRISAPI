using HRISAPI.Application.Repositories;
using HRISAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRISAPI.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MyDbContext _db;
        private readonly DbSet<T> _dbSet;
        public Repository(MyDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }
        public async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }
        public async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return true;
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> expression, string? includeProperties = null)
        {
            IQueryable<T> entities = _dbSet;

            // Include navigation properties
            if (!string.IsNullOrEmpty(includeProperties))
            {
                string[] includeProps = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includeProps)
                {
                    entities = entities.Include(include);
                }
            }

            // Apply the filter expression and return the first or default result
            return await entities.Where(expression).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> queries = _dbSet.Where(expression);
            return await queries.ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<T> entities = _dbSet;
            // Include navigation properties
            if (!string.IsNullOrEmpty(includeProperties))
            {
                string[] includeProps = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includeProps)
                {
                    entities = entities.Include(include);
                }
            }
            return await entities.ToListAsync();
        }
        public bool Remove(T entity)
        {
            _dbSet.Remove(entity);
            return true;
        }
        public bool RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            return true;
        }
        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
