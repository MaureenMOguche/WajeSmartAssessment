using Moq;
using Microsoft.EntityFrameworkCore;
using WajeSmartAssessment.Infrastructure;
using WajeSmartAssessment.Infrastructure.Repositories;
using System.Linq.Expressions;

public abstract class RepositoryTestsSetupBase<T> where T : class
{
    protected Mock<ApplicationDbContext> MockContext { get; }
    protected UnitOfWork UnitOfWork { get; }
    protected List<T> Entities { get; }

    protected RepositoryTestsSetupBase()
    {
        MockContext = new Mock<ApplicationDbContext>();
        UnitOfWork = new UnitOfWork(MockContext.Object);
        Entities = new List<T>();
    }

    protected Mock<DbSet<TEntity>> SetupMockSet<TEntity>(List<TEntity> data) where TEntity : class
    {
        var queryableData = data.AsQueryable();
        var mockSet = new Mock<DbSet<TEntity>>();
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());
        mockSet.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Callback<TEntity,
            CancellationToken>((entity, token) => data.Add(entity));
        mockSet.Setup(m => m.Remove(It.IsAny<TEntity>())).Callback<TEntity>((entity) => data.Remove(entity));

        //mockSet.Setup(m => m.Where(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>())
        //    .ReturnsAsync((Expression<Func<TEntity, bool>> predicate, CancellationToken token) => data.Where(predicate)));

        //mockSet.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);
        //.Callback<Func<TEntity, bool>>(predicate => data.Any(predicate));

        return mockSet;
    }

    protected void SetupMockDbSet()
    {
        var mockSet = SetupMockSet(Entities);
        MockContext.Setup(c => c.Set<T>()).Returns(mockSet.Object);
    }
}