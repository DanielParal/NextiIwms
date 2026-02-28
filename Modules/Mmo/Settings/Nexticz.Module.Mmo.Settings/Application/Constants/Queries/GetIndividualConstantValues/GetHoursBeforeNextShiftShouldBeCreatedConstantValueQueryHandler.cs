using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetIndividualConstantValues;

internal class GetHoursBeforeNextShiftShouldBeCreatedConstantValueQueryHandler(
    ISender sender,
    ILogger<GetHoursBeforeNextShiftShouldBeCreatedConstantValueQueryHandler> logger) : IRequestHandler<GetHoursBeforeNextShiftShouldBeCreatedConstantValueQuery, int>
{
    public async Task<int> Handle(GetHoursBeforeNextShiftShouldBeCreatedConstantValueQuery request, CancellationToken cancellationToken)
    {
        var mmoConstant = 
            await sender.Send(new GetConstantByKeyQuery(ConstantNames.HoursBeforeNextShiftShouldBeCreated), cancellationToken);
        
        if (mmoConstant.IsError)
        {
            logger.LogError("Object {ObjectName} with key: {Key} does not exist. We cannot retrieve constant.", 
                nameof(Constant), ConstantNames.HoursBeforeNextShiftShouldBeCreated);
            throw new Exception($"Constant {ConstantNames.HoursBeforeNextShiftShouldBeCreated} does not exist.");
        }

        if (!int.TryParse(mmoConstant.Value.Value, out var result))
        {
            logger.LogError("Failed to parse value of {ObjectName} with key: {Key} to type integer. Value is: {ConstantValue} and type is: {ConstantType}.",
                nameof(Constant), ConstantNames.HoursBeforeNextShiftShouldBeCreated, 
                mmoConstant.Value.Value, mmoConstant.Value.Type.ToString());
            throw new Exception($"Cannot parse constant {ConstantNames.HoursBeforeNextShiftShouldBeCreated} value to integer.");
        }
        
        return result; 
    }
}