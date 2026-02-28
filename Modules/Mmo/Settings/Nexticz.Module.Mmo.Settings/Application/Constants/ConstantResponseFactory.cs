using Nexticz.Module.Mmo.Settings.Contracts.Constants;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants;

internal class ConstantResponseFactory
{
    public static ConstantResponse Create(Constant constant)
    {
        return new ConstantResponse(
            constant.Id,
            constant.Key,
            constant.Value,
            (ConstantTypeContract)constant.Type,
            constant.Description ?? string.Empty);
    }
}