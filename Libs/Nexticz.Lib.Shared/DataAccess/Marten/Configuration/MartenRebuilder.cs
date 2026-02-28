using System.Reflection;
using Marten;

namespace Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

public abstract class MartenRebuilder
{
    public static async Task<bool> RebuildProjectionAsync(IDocumentStore documentStore, Assembly targetAssembly, 
        string projectionTypeString, CancellationToken cancellationToken)
    {
        await documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        using var daemon = await documentStore.BuildProjectionDaemonAsync();
        
        var projectionType = targetAssembly.GetType(projectionTypeString);
        
        if (projectionType is null)
            return false;
                
        await daemon.RebuildProjectionAsync(projectionType, cancellationToken);

        return true;
    }
}