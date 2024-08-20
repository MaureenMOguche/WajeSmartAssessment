
using WajeSmartAssessment.Application.Contracts.Repository;

namespace WajeSmartAssessment.Infrastructure.Repositories;
public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];
    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)) && _repositories[typeof(T)] != null)
        {
            if (_repositories[typeof(T)] is IGenericRepository<T> repo) return repo;
        }
        var repository = new GenericRepository<T>(dbContext);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }


}
