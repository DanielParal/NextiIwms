using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.DevExtreme;


namespace Nexticz.Module.Mmo.Settings.Application.Seeds.SeedHandlers;

internal abstract class BaseSeedHandler<TQuery, TEntity>(ISender sender, ILogger logger)
    where TQuery : IRequest<FilteredResult<TEntity>>
    where TEntity : class

{
    protected async Task<bool> EnsureNoExistingDataAsync(
        TQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);

        if (result.Data.Count != 0)
        {
            logger.LogInformation("There are already data in {ObjectName} projection. Nothing to seed.", typeof(TEntity).Name);
            return false;
        }

        return true;
    }

}