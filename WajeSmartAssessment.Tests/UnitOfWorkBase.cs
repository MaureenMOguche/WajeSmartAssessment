using Moq;
using System.Linq.Expressions;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Infrastructure;
using WajeSmartAssessment.Tests;
using WajeSmartAssessment.Tests.Helpers;

public abstract class UnitOfWorkTestsSetupBase
{
    protected Mock<ApplicationDbContext> MockContext { get; }
    protected Mock<IUnitOfWork> MockUnitOfWork { get; }
    protected Dictionary<Type, object> MockRepositories { get; }
    protected Dictionary<Type, object> EntityLists { get; }

    protected UnitOfWorkTestsSetupBase()
    {
        MockContext = new Mock<ApplicationDbContext>();
        MockUnitOfWork = new Mock<IUnitOfWork>();
        MockRepositories = new Dictionary<Type, object>();
        EntityLists = new Dictionary<Type, object>();
    }

    protected void SetupMockRepository<TEntity>() where TEntity : class
    {
        var entityList = new List<TEntity>();
        EntityLists[typeof(TEntity)] = entityList;

        var mockRepo = new Mock<IGenericRepository<TEntity>>();

        //    mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<bool>()))
        //.Returns((Expression<Func<TEntity, bool>> filter, bool trackChanges) =>
        //{
        //    IQueryable<TEntity> query = entityList.AsQueryable();
        //    if (filter != null)
        //    {
        //        query = query.Where(filter);
        //    }
        //    return query;
        //});


        mockRepo.Setup(repo => repo.GetQueryable(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<bool>()))
            .Returns((Expression<Func<TEntity, bool>> filter, bool trackChanges) =>
            {
                IQueryable<TEntity> query = new AsyncEnumerable<TEntity>(entityList);
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                return query;
            });

        mockRepo.Setup(repo => repo.GetById(It.IsAny<string>()))
           .ReturnsAsync((Guid id) => entityList.FirstOrDefault(e => GetEntityId(e).Equals(id)));

        mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TEntity>()))
            .Returns((TEntity entity) => {
                entityList.Add(entity);
                return Task.FromResult(entity);
            });

        //mockRepo.Setup(repo => repo.EntityExists(It.IsAny<Expression<Func<TEntity, bool>>>()))
        //    .ReturnsAsync((Expression<Func<TEntity, bool>> predicate) => entityList.Any(predicate.Compile()));

        mockRepo.Setup(repo => repo.EntityExists(It.IsAny<Expression<Func<TEntity, bool>>>()))
            .ReturnsAsync((Expression<Func<TEntity, bool>> predicate) => entityList.AsQueryable().Any(predicate));


        MockRepositories[typeof(TEntity)] = mockRepo;

        MockUnitOfWork.Setup(uow => uow.GetRepository<TEntity>())
            .Returns(mockRepo.Object);


        SetupSaveChangesAsync();
    }

    private Guid GetEntityId<TEntity>(TEntity entity) where TEntity : class
    {
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            return (Guid)idProperty.GetValue(entity);
        }
        throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have a Guid Id property.");
    }

    protected Mock<IGenericRepository<TEntity>> GetMockRepository<TEntity>() where TEntity : class
    {
        return (Mock<IGenericRepository<TEntity>>)MockRepositories[typeof(TEntity)];
    }

    protected List<TEntity> GetEntityList<TEntity>() where TEntity : class
    {
        return (List<TEntity>)EntityLists[typeof(TEntity)];
    }

    protected void AddEntity<TEntity>(TEntity entity) where TEntity : class
    {
        var entityList = GetEntityList<TEntity>();
        entityList.Add(entity);
    }

    protected void SetupSaveChangesAsync(bool returnValue = true)
    {
        MockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(returnValue);
    }

    // New method to verify SaveChangesAsync was called
    protected void VerifySaveChangesAsyncCalled(Times times)
    {
        MockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), times);
    }
}