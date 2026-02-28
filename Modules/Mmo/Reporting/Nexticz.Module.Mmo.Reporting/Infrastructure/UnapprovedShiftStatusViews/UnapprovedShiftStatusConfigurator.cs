using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.UnapprovedShiftStatusViews;

internal class UnapprovedShiftStatusConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<UnapprovedShiftStatusProjection>(ProjectionLifecycle.Inline);
        options.Schema.For<UnapprovedShiftStatusView>()
            .Index(x => x.ShiftId);
        options.Schema.For<UnapprovedShiftStatusView>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<UnapprovedShiftStatusView>());
    }
}