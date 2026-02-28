using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;


namespace Nexticz.Module.Mmo.Settings.Infrastructure.Users;

internal class UserConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<UserProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<User>().Index(x => x.ReceivableNotifications);
        
        options.Schema.For<User>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<User>());
    }
}