using Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WajeSmartAssessment.Infrastructure;
using WajeSmartAssessment.Application.Contracts.Repository;

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

        var mockSet = SetupMockSet(entityList);
        MockContext.Setup(c => c.Set<TEntity>()).Returns(mockSet.Object);

        var mockRepo = new Mock<IGenericRepository<TEntity>>();
        mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<bool>()))
            .Returns((Expression<Func<TEntity, bool>> filter, bool trackChanges) =>
                entityList.AsQueryable().Where(filter));

        mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TEntity>()))
            .Returns((TEntity entity) => {
                entityList.Add(entity);
                return Task.FromResult(entity);
            });

        mockRepo.Setup(repo => repo.EntityExists(It.IsAny<Expression<Func<TEntity, bool>>>()))
            .Returns((Func<TEntity, bool> predicate) =>
                Task.FromResult(entityList.FirstOrDefault(predicate) != null));

        MockRepositories[typeof(TEntity)] = mockRepo;

        MockUnitOfWork.Setup(uow => uow.GetRepository<TEntity>())
            .Returns(mockRepo.Object);
    }

    private Mock<DbSet<TEntity>> SetupMockSet<TEntity>(List<TEntity> data) where TEntity : class
    {
        var queryableData = data.AsQueryable();
        var mockSet = new Mock<DbSet<TEntity>>();
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());
        return mockSet;
    }

    protected Mock<IGenericRepository<TEntity>> GetMockRepository<TEntity>() where TEntity : class
    {
        return (Mock<IGenericRepository<TEntity>>)MockRepositories[typeof(TEntity)];
    }

    protected List<TEntity> GetEntityList<TEntity>() where TEntity : class
    {
        return (List<TEntity>)EntityLists[typeof(TEntity)];
    }
}