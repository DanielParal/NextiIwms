using System.Linq.Expressions;
using JasperFx.Events;
using Nexticz.Lib.Shared.DevExtreme;

namespace Nexticz.Lib.Shared.DataAccess.Marten;

public interface IMartenReadOnlyEventStoreRepository
{
    Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken) where T : notnull;
    Task<T?> GetFirstByConditionAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellationToken) where T : notnull;
    Task<IReadOnlyList<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T : notnull;
    Task<IReadOnlyList<T>> GetAllByConditionAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellationToken) where T : notnull;
    Task<IReadOnlyList<T>> GetAllByConditionAsync<T>(Expression<Func<T, bool>> condition, int take, CancellationToken cancellationToken) where T : notnull;
    Task<FilteredResult<T>> GetFilteredAsync<T>(BaseFilteringParams filteringParams,
        CancellationToken cancellationToken);

    Task<FilteredResult<T>> GetFilteredAsync<T>(IQueryable<T> query, BaseFilteringParams filteringParams,
        CancellationToken cancellationToken);
    Task<IEnumerable<object>> GetGroupedResultAsync<T, TResult>(Expression<Func<T, TResult>> selector, int skip, int? take, CancellationToken cancellationToken) where T : notnull where TResult : notnull;
    Task<IEnumerable<object>> GetGroupedEnumResultAsync<T>(Expression<Func<T, object>> selector, int skip, int? take, CancellationToken cancellationToken) where T : notnull;
    Task<FilteredResult<IEvent>> GetFilteredEventsByStreamIdAsync(Guid streamId,
        HistoryEventsFilteringParams filteringParams,
        CancellationToken cancellationToken);
    Task<bool> DoesStreamExistAsync(Guid streamId, CancellationToken cancellationToken);
}