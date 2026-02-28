using Nexticz.Module.Sign.Settings.Contracts.Constants;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Constants;

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