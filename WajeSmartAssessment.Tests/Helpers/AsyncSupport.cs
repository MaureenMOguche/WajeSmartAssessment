﻿using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace WajeSmartAssessment.Tests.Helpers;


public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IQueryable<T> _innerQueryable;

    public AsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
        _innerQueryable = enumerable.AsQueryable();
    }

    public AsyncEnumerable(Expression expression) : base(expression)
    {
        _innerQueryable = new EnumerableQuery<T>(expression);
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(_innerQueryable.Provider);
}

public class AsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;
    public AsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
    public T Current => _inner.Current;
    //public ValueTask<bool> MoveNextAsync() => MoveNextAsync();
    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new ValueTask();
    }
}

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression)
        => new AsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new AsyncEnumerable<TElement>(expression);

    public object Execute(Expression expression)
        => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: new[] { typeof(Expression) })
            .MakeGenericMethod(expectedResultType)
            .Invoke(this, new[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, new[] { executionResult });
    }
}


//public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
//{
//    private readonly IQueryProvider _inner;

//    internal TestAsyncQueryProvider(IQueryProvider inner)
//    {
//        _inner = inner;
//    }

//    public IQueryable CreateQuery(Expression expression)
//    {
//        return new TestAsyncEnumerable<TEntity>(expression);
//    }

//    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
//    {
//        return new TestAsyncEnumerable<TElement>(expression);
//    }

//    public object Execute(Expression expression)
//    {
//        return _inner.Execute(expression);
//    }

//    public TResult Execute<TResult>(Expression expression)
//    {
//        return _inner.Execute<TResult>(expression);
//    }

//    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
//    {
//        return Execute<TResult>(expression);
//    }
//}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider
    {
        get { return new TestAsyncQueryProvider<T>(this); }
    }
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current
    {
        get
        {
            return _inner.Current;
        }
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new ValueTask();
    }
}