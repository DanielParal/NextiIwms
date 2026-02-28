using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Portal.Infrastructure.Modules;

internal class ModuleConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<ModuleProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<Domain.ModuleAggregate.Module>().UniqueIndex(x => x.Name);
        options.Schema.For<Domain.ModuleAggregate.Module>().Index(x => x.SortOrder);
        
        options.Schema.For<Domain.ModuleAggregate.Module>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<Domain.ModuleAggregate.Module>());
    }
}