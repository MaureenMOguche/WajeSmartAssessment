using System.Linq.Expressions;

namespace WajeSmartAssessment.Application.Contracts.Repository;
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetById(string id);
    IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null, bool trackChanges = false);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task AddBulkAsync(IEnumerable<T> entities);
    void DeleteBulk(IEnumerable<T> entities);
    Task<bool> EntityExists(Expression<Func<T, bool>> filter);
}
