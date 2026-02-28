using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Printers;

internal class PrinterConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<PrinterProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Printer>()
            .Index(x => x.Code, 
                idx =>
                {
                    idx.IsUnique = true;
                });
        
        options.Schema.For<Printer>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Printer>());
    }
}