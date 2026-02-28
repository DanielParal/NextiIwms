using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetIndividualConstantValues;

internal class GetSpaceBetweenPackagingsOnWashingMachineConstantValueQueryHandler(
    ISender sender,
    ILogger<GetSpaceBetweenPackagingsOnWashingMachineConstantValueQueryHandler> logger
    )
    : IRequestHandler<GetSpaceBetweenPackagingsOnWashingMachineConstantValueQuery, int>
{
    public async Task<int> Handle(GetSpaceBetweenPackagingsOnWashingMachineConstantValueQuery request, CancellationToken cancellationToken)
    {
        var mmoConstant = 
            await sender.Send(new GetConstantByKeyQuery(ConstantNames.SpaceBetweenPackagingsOnWashingMachine), cancellationToken);
        
        if (mmoConstant.IsError)
        {
            logger.LogError("Object {ObjectName} with key: {Key} does not exist. We cannot retrieve constant.", 
                nameof(Constant), ConstantNames.SpaceBetweenPackagingsOnWashingMachine);
            throw new Exception($"Constant {ConstantNames.SpaceBetweenPackagingsOnWashingMachine} does not exist.");
        }

        if (!int.TryParse(mmoConstant.Value.Value, out var result))
        {
            logger.LogError("Failed to parse value of {ObjectName} with key: {Key} to type integer. Value is: {ConstantValue} and type is: {ConstantType}.",
                nameof(Constant), ConstantNames.SpaceBetweenPackagingsOnWashingMachine, 
                mmoConstant.Value.Value, mmoConstant.Value.Type.ToString());
            throw new Exception($"Cannot parse constant {ConstantNames.SpaceBetweenPackagingsOnWashingMachine} value to integer.");
        }
        
        return result;
    }
}