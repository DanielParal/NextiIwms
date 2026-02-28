using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.Views;


namespace Nexticz.Module.Mmo.Reporting.Infrastructure.LineItemViews;

internal class LineItemConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<LineItemProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<LineItemView>().Index(x => x.ShiftId);
        options.Schema.For<LineItemView>().Index(x => x.Type);
        options.Schema.For<LineItemView>().Index(x => x.LineCode);
        options.Schema.For<LineItemView>().Index(x => x.IsPlanned);
        options.Schema.For<LineItemView>().Index(x => x.KitId);
        options.Schema.For<LineItemView>().Index(x => new { x.StartDate, x.EndDate });
        
        options.Schema.For<LineItemView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<LineItemView>());
    }
}