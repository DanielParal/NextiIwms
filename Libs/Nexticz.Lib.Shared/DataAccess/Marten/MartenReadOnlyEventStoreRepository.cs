using System.Linq.Expressions;
using DevExtreme.AspNet.Data;
using JasperFx.Events;
using Marten;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Lib.Shared.DataAccess.Marten;

public abstract class MartenReadOnlyEventStoreRepository(IMartenDocumentSessionProvider documentSessionProvider)
    : IMartenReadOnlyEventStoreRepository
{
    private readonly IQuerySession _session = documentSessionProvider.GetQuerySession();

    public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken) where T : notnull
    {
        return await _session.LoadAsync<T>(id, cancellationToken);
    }

    public async Task<T?> GetFirstByConditionAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellationToken) where T : notnull
    {
        return await _session
            .Query<T>()
            .Where(condition)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T : notnull
    {
        return await _session
            .Query<T>()
            .ApplyDefaultOrdering()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllByConditionAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellationToken) where T : notnull
    {
        return await _session
            .Query<T>()
            .Where(condition)
            .ApplyDefaultOrdering()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllByConditionAsync<T>(Expression<Func<T, bool>> condition, int take, CancellationToken cancellationToken) where T : notnull
    {
        return await _session
            .Query<T>()
            .Where(condition)
            .ApplyDefaultOrdering()
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<FilteredResult<T>> GetFilteredAsync<T>(BaseFilteringParams filteringParams, CancellationToken cancellationToken)
    {
        IQueryable<T> query = _session.Query<T>();
        
        if (!filteringParams.IsOrderApplied)
            query = query.ApplyDefaultOrdering();
        
        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
    
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult<T>();
    }
    
    public async Task<FilteredResult<T>> GetFilteredAsync<T>(IQueryable<T> query, BaseFilteringParams filteringParams, CancellationToken cancellationToken)
    {
        var hasOrderBy = query.Expression.ToString().Contains("OrderBy");
        if (!hasOrderBy && !filteringParams.IsOrderApplied)
            query = query.ApplyDefaultOrdering();
        
        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
    
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult<T>();
    }
    
    public async Task<IEnumerable<object>> GetGroupedResultAsync<T, TResult>(Expression<Func<T, TResult>> selector, int skip, int? take, CancellationToken cancellationToken) where T : notnull where TResult : notnull
    {
        var resultQueryable = _session.Query<T>()
            .Select(selector)
            .Distinct()
            .Skip(skip);
        
        if (take is not null)
            resultQueryable = resultQueryable.Take(take.Value);
        
        var result = await resultQueryable.ToListAsync(token: cancellationToken);
            
        return result
            .Select(x => new { Key = x });
    }

    public async Task<IEnumerable<object>> GetGroupedEnumResultAsync<T>(Expression<Func<T, object>> selector, int skip, int? take,
        CancellationToken cancellationToken) where T : notnull
    {
        var resultQueryable = _session.Query<T>()
            .Select(selector)
            .Distinct()
            .Skip(skip);
        
        if (take is not null)
            resultQueryable = resultQueryable.Take(take.Value);
        
        var arrayInString = await resultQueryable.ToJsonArray(cancellationToken);
        
        var arrayOfStrings = arrayInString
            .Trim('[', ']')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToArray();
        
        return arrayOfStrings
            .Select(x => new { Key = x });
    }
    
    public async Task<FilteredResult<IEvent>> GetFilteredEventsByStreamIdAsync(Guid streamId, HistoryEventsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var queryable = _session.Events
            .QueryRawEventDataOnly<IEvent>()
            .Where(e => e.StreamId == streamId);
    
        if (filteringParams.StartDate.HasValue)
        {
            queryable = queryable.Where(e => e.Timestamp >= filteringParams.StartDate.Value);
        }

        if (filteringParams.EndDate.HasValue)
        {
            queryable = queryable.Where(e => e.Timestamp <= filteringParams.EndDate.Value);
        }
        
        queryable = queryable.OrderByDescending(e => e.Timestamp);
        
        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
    
        var loadResult = await DataSourceLoader.LoadAsync(queryable, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult<IEvent>();
    }
    
    public async Task<bool> DoesStreamExistAsync(Guid streamId, CancellationToken cancellationToken)
    {
        return await _session.Events.FetchStreamStateAsync(streamId, cancellationToken) != null;
    }
}