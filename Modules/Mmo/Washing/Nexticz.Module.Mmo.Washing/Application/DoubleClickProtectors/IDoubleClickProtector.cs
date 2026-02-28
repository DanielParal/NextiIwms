using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.DoubleClickProtectors;

internal interface IDoubleClickProtector
{
    Task<bool> CanFinishKitAsync(Batch batch, string worker);
}