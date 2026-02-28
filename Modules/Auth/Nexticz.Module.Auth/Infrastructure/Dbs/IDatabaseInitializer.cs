namespace Nexticz.Module.Auth.Infrastructure.Dbs;

internal interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}