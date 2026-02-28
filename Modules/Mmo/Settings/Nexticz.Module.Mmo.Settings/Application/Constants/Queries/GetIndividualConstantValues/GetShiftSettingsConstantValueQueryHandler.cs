using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetIndividualConstantValues;

internal class GetShiftSettingsConstantValueQueryHandler(
    ISender sender,
    ILogger<GetShiftSettingsConstantValueQueryHandler> logger) 
    : IRequestHandler<GetShiftSettingsConstantValueQuery, string>
{
    public async Task<string> Handle(GetShiftSettingsConstantValueQuery request, CancellationToken cancellationToken)
    {
        var mmoConstant = 
            await sender.Send(new GetConstantByKeyQuery(ConstantNames.ShiftSettings), cancellationToken);
        
        if (mmoConstant.IsError)
        {
            logger.LogError("Object {ObjectName} with key: {Key} does not exist. We cannot retrieve constant.", 
                nameof(Constant), ConstantNames.ShiftSettings);
            throw new Exception($"Constant {ConstantNames.ShiftSettings} does not exist.");
        }
        
        return mmoConstant.Value.Value;
    }
}