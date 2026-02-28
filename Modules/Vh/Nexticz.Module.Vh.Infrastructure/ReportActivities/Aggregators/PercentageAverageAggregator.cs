using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;

namespace Nexticz.Module.Vh.Infrastructure.ReportActivities.Aggregators;

internal class PercentageAverageAggregator<T> : Aggregator<T>
{
    private decimal _durationTime;
    private decimal _score;

    public PercentageAverageAggregator(IAccessor<T> accessor) : base(accessor)
    {
    }

    public override void Step(T container, string selector)
    {
        _score += Convert.ToDecimal(Accessor.Read(container, "Score"));
        _durationTime += Convert.ToDecimal(Accessor.Read(container, "DurationTime"));
    }

    public override object Finish()
    {
        var percentage = _durationTime != 0
            ? _score * 100 / _durationTime
            : 0;

        return percentage.ToString("0.00") + " %";
    }
}