using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetIndividualConstantValues;

internal class GetBreakSettingsConstantValueQueryHandler(
    ISender sender,
    ILogger<GetBreakSettingsConstantValueQueryHandler> logger) 
    : IRequestHandler<GetBreakSettingsConstantValueQuery, string>
{
    public async Task<string> Handle(GetBreakSettingsConstantValueQuery request, CancellationToken cancellationToken)
    {
        var mmoConstant = 
            await sender.Send(new GetConstantByKeyQuery(ConstantNames.BreakSettings), cancellationToken);
        
        if (mmoConstant.IsError)
        {
            logger.LogError("Object {ObjectName} with key: {Key} does not exist. We cannot retrieve constant.", 
                nameof(Constant), ConstantNames.BreakSettings);
            throw new Exception($"Constant {ConstantNames.BreakSettings} does not exist.");
        }
        
        return mmoConstant.Value.Value;
    }
}