using JasperFx.Events.Projections;
using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;
using Weasel.Postgresql.Tables;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Users;

internal class UserConfigurator : IMartenConfigurator
{
    public void Configure(StoreOptions options)
    {
        options.Projections.Add<UserProjection>(ProjectionLifecycle.Inline);
        
        options.Schema.For<User>()
            .Index(x => x.Id, 
                idx =>
                {
                    idx.IsUnique = true;
                })
            .Index(x => x.UserName, 
                idx =>
                {
                    idx.IsUnique = true;
                })
            .Index(x => x.DepositorCodes.Select(code => code.ToUpperInvariant()), 
                idx =>
                {
                    idx.Method = IndexMethod.gin;
                })
            .Index(x => x.SigningDeviceCodes.Select(code => code.ToUpperInvariant()), 
                idx =>
                {
                    idx.Method = IndexMethod.gin;
                })
            .Index(x => x.DepositorGroupCodes.Select(code => code.ToUpperInvariant()), 
                idx =>
                {
                    idx.Method = IndexMethod.gin;
                });
        
        options.Schema.For<User>().DocumentAlias(MartenConfigurationOrchestrator.PluralizeTableName<User>());
    }
}