using System.Linq.Expressions;
using System.Reflection;

namespace Nexticz.Lib.Shared.DataAccess;

public static class QueryableExtensions
{
    private const string SortOrderPropertyName = "SortOrder";
    private const string CreatedAtPropertyName = "CreatedAt";
    private const string DateCreatedPropertyName = "DateCreated";
    private const string IdPropertyName = "Id";
    
    /// <summary>
    /// Order priority: SortOrder -> CreatedAt -> Id
    /// We need to build expression: x => x.Property
    /// We use standard LINQ OrderByDescending
    /// If no properties found, return query unchanged
    /// </summary>
    public static IQueryable<T> ApplyDefaultOrdering<T>(this IQueryable<T> query)
    {
        // var alreadyOrdered = query is IOrderedQueryable<T>;
        // if (alreadyOrdered)
        //     return query;
        
        var type = typeof(T);

        // Priority order: SortOrder (asc) -> CreatedAt (desc) -> DateCreated (desc) -> Id (desc)
        var propertyNames = new[] { SortOrderPropertyName, CreatedAtPropertyName, DateCreatedPropertyName, IdPropertyName };

        foreach (var propName in propertyNames)
        {
            var property = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null) 
                continue;
            
            // Build expression: x => x.Property
            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);
            
            // Choose ascending for SortOrder, descending otherwise
            var ascending = string.Equals(propName, SortOrderPropertyName, StringComparison.OrdinalIgnoreCase);
            var methodName = ascending ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending);
            
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(type, property.PropertyType);

            var ordered = (IQueryable<T>)method.Invoke(null, [query, lambda])!;
            
            return ordered;
        }
        
        return query;
    }
}