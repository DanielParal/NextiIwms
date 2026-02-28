using System.Linq.Expressions;
using System.Text.Json;
using ErrorOr;

namespace Nexticz.Lib.Shared.DevExtreme.Grouping;

public abstract class GroupLambdaExpressionFactory
{
    private GroupLambdaExpressionFactory()
    {
    }

    public static ErrorOr<LambdaExpression> Create<T>(string? group) where T : notnull
    {
        if (string.IsNullOrWhiteSpace(group))
        {
            return Error.Failure($"{nameof(GroupLambdaExpressionFactory)}.{nameof(Create)}.FilteringParamsGroupIsNullOrEmpty", "FilteringParams.Group is null or empty.");
        }
        
        var groupDescriptors = JsonSerializer.Deserialize<List<GroupDescriptor>>(
            group,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (groupDescriptors is { Count: 0 })
        {
            return Error.Failure($"{nameof(GroupLambdaExpressionFactory)}.{nameof(Create)}.NoGroupDescriptorFound", "No group descriptors found.");
        }
        
        return CreateLambdaExpression<T>(groupDescriptors![0].Selector);
    }
    
    private static ErrorOr<LambdaExpression> CreateLambdaExpression<T>(string propertyPath)
        where T : notnull
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression currentExpression = parameter;
        
        var propertyNames = propertyPath.Split('.');
        
        var currentType = typeof(T);
        foreach (var propertyName in propertyNames)
        {
            var propertyInfo = currentType
                .GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null)
            {
                return Error.NotFound(
                    $"{nameof(GroupLambdaExpressionFactory)}.PropertyNameNotFound",
                    $"Property '{propertyName}' does not exist on type {currentType.Name}");
            }
            
            currentExpression = Expression.Property(currentExpression, propertyInfo);
            currentType = propertyInfo.PropertyType;
        }
        
        return Expression.Lambda(currentExpression, parameter);
    }
}