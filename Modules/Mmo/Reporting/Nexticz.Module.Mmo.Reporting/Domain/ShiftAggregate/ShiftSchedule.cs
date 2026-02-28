using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

public class ShiftSchedule : ValueObject
{
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }

    public ShiftSchedule(DateTimeOffset start, DateTimeOffset end)
    {
        if (start.CompareTo(end) >= 0)
            throw new ArgumentException($"Start ({start}) cannot be greater than or equal to end ({end}).", nameof(start));
        
        Start = start;
        End = end;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}