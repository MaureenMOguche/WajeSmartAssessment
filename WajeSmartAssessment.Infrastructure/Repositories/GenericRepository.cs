using Microsoft.EntityFrameworkCore;
using WajeSmartAssessment.Application.Contracts.Repository;
using System.Linq.Expressions;

namespace WajeSmartAssessment.Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        var ent = await _dbSet.AddAsync(entity);
        //return ent.Entity;
    }

    public async Task AddBulkAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteBulk(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<bool> EntityExists(Expression<Func<T, bool>> filter)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(filter);
        return entity != null;
    }
    
    //public async Task<bool> EntityExists(Expression<Func<T, bool>> filter)
    //{
    //    return await _dbSet.AnyAsync(filter);
    //}

    public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null, bool trackChanges = false)
    {
        IQueryable<T> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (!trackChanges)
            return query.AsNoTracking();
        return query;
    }

    public async Task<T?> GetById(string id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}

