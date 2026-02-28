using System.Linq.Expressions;
using DevExtreme.AspNet.Data.Async;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public class MartenAsyncAdapter : IAsyncAdapter
{
    public Task<int> CountAsync(IQueryProvider queryProvider, Expression expr, CancellationToken cancellationToken)
    {
        var count = queryProvider.Execute<int>(expr);
        return Task.FromResult(count);
    }

    public Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider queryProvider, Expression expr, CancellationToken cancellationToken)
    {
        var queryable = queryProvider.CreateQuery<T>(expr);
        return Task.FromResult<IEnumerable<T>>(queryable.ToList());
    }
}