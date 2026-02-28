using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;


namespace Nexticz.Module.Mmo.Reporting.Infrastructure.Shifts;

internal class ShiftConfiguration : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ShiftProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<Shift>().Index(x => new { x.Schedule.Start, x.Schedule.End });
        options.Schema.For<Shift>().Index(x => x.IsLast);
        options.Schema.For<Shift>().Index(x => x.IsNextToLast);
        options.Schema.For<Shift>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Shift>());
    }
}