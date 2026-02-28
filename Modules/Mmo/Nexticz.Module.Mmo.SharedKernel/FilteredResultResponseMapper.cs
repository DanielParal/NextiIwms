namespace Nexticz.Module.Mmo.SharedKernel;

public static class FilteredResultResponseMapper
{
    public static dynamic? MapFrom<T, T2>(dynamic source, Func<T, T2> convertFunc)
    {
        if (source != null && source.GetType() == typeof(List<T>))
        {
            return source
                .OfType<T>()
                .Select(convertFunc);
        }
        
        return source;
    }
}