using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Lib.Shared.DevExtreme.Grouping;

public abstract class GroupedResultHandler(
    ILogger<GroupedResultHandler> logger,
    IMartenReadOnlyEventStoreRepository martenReadOnlyEventStoreRepository) 
    : IGroupedResultHandler
{
    public async Task<FilteredResult<object>> GetGroupedResultAsync<T>(BaseFilteringParams filteringParams, CancellationToken cancellationToken) where T : notnull
    {
        var lambdaExpression = GroupLambdaExpressionFactory.Create<T>(filteringParams.Group);
        var skip = int.TryParse(filteringParams.Skip, out var skipValue) ? skipValue : 0;
        int? take = int.TryParse(filteringParams.Take, out var takeValue) ? takeValue : null;
        
        if (lambdaExpression.IsError)
        {
            logger.LogWarning("Problem creating selector. Returning empty list. ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}",
                lambdaExpression.FirstError.Code, lambdaExpression.FirstError.Description);
            return new FilteredResult<object>
            {
                Data = [],
                TotalCount = -1,
                GroupCount = -1
            };
        }
        
        IEnumerable<object> data;
        switch (lambdaExpression.Value.ReturnType)
        {
            case { } t when t == typeof(string):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, string>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(int):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, int>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(long):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, long>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(double):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, double>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(decimal):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, decimal>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(float):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, float>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(bool):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, bool>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { } t when t == typeof(DateTimeOffset):
                data = await martenReadOnlyEventStoreRepository.GetGroupedResultAsync((Expression<Func<T, DateTimeOffset>>)lambdaExpression.Value, skip, take, cancellationToken);
                break;
            case { IsEnum: true }:
                {
                    var convertedExpression = Expression.Lambda<Func<T, object>>(
                        Expression.Convert(lambdaExpression.Value.Body, typeof(object)),
                        lambdaExpression.Value.Parameters);
                    
                    data = await martenReadOnlyEventStoreRepository.GetGroupedEnumResultAsync(
                        convertedExpression, skip, take, cancellationToken);
                    break;
                }
            default:
                data = [];
                break;
        }
        
        var dataResult = data.ToList();
        return new FilteredResult<object>
        {
            Data = dataResult,
            TotalCount = dataResult.Count,
            GroupCount = dataResult.Count
        };
    }
}