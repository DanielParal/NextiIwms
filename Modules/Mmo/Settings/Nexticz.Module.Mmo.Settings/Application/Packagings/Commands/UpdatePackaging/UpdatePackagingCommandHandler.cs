using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachinesByCodes;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.UpdatePackaging;

internal class UpdatePackagingCommandHandler (
    ILogger<UpdatePackagingCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<UpdatePackagingCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePackagingCommand request, CancellationToken cancellationToken)
    {
        var packaging = await sender.Send(new GetPackagingByCodeQuery(request.Code), cancellationToken);
        
        var validationResult = await IsValidAsync(packaging, request, cancellationToken);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var washingMachineSpeeds = request.UpdatePackagingRequest.WashingMachineSpeeds.Select(x => new WashingMachineSpeed(x.WashingMachineCode, (WashingMachineSpeedLevel)x.Speed)).ToArray();
        var packagingUpdatedEvent = new PackagingUpdatedEvent(
            packaging.Value.Id, request.Code, request.UpdatePackagingRequest.PackagingTypeCode, request.UpdatePackagingRequest.PackagingCirculationCode, 
            request.UpdatePackagingRequest.Name, request.UpdatePackagingRequest.MustBeWashed, 
            new Dimensions(request.UpdatePackagingRequest.Dimensions.Depth, request.UpdatePackagingRequest.Dimensions.Width, request.UpdatePackagingRequest.Dimensions.Height), 
            request.UpdatePackagingRequest.Weight, washingMachineSpeeds
        );
        
        unitOfWork.AppendEvent(packaging.Value.Id, packagingUpdatedEvent);

        return Result.Updated;
    }
    
    private async Task<ErrorOr<Success>> IsValidAsync(ErrorOr<Packaging> packaging, UpdatePackagingCommand request, CancellationToken cancellationToken)
    {
        if (!packaging.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with Code: {Code}. Nothing to update.", nameof(Packaging), request.Code);
            return packaging.Errors;
        }
        
        var packagingType = await sender.Send(new GetPackagingTypeByCodeQuery(request.UpdatePackagingRequest.PackagingTypeCode), cancellationToken);
        if (packagingType.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(PackagingType), request.UpdatePackagingRequest.PackagingTypeCode);
            return PackagingErrors.ValidationPackagingTypeDoesNotExist;
        }
        
        var packagingCirculation = await sender.Send(new GetPackagingCirculationByCodeQuery(request.UpdatePackagingRequest.PackagingCirculationCode), cancellationToken);
        if (packagingCirculation.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(PackagingCirculation), request.UpdatePackagingRequest.PackagingCirculationCode);
            return PackagingErrors.ValidationPackagingCirculationDoesNotExist;
        }

        var codesFromRequest = request.UpdatePackagingRequest.WashingMachineSpeeds.Select(x => x.WashingMachineCode).ToArray();
        var washingMachinesFromRequest = await sender.Send(new GetWashingMachinesByCodesQuery(codesFromRequest), cancellationToken);
        var washingMachines = await sender.Send(new GetWashingMachinesQuery(new BaseFilteringParams()), cancellationToken);
        if (washingMachinesFromRequest.Count != washingMachines.Data.Count || codesFromRequest.Length > washingMachines.Data.Count)
        {
            var washingMachineCodesCommaSeparated = string.Join(", ", washingMachines.Data.Select(x => x.Code));
            var specifiedCodesCommaSeparated = string.Join(", ", codesFromRequest);
            logger.LogInformation("{ObjectName} with code: {Code} does not provide the correct number of speeds for machine machines. " +
                                  "Provided codes: {ProvidedCodes}. " +
                                  "You need to specify speeds for machine codes: {WashingMachineCodes}. Nothing to create.", 
                nameof(Packaging), packaging.Value.Code, specifiedCodesCommaSeparated, washingMachineCodesCommaSeparated);
            return PackagingErrors.ValidationPackagingSpeedsCountMismatch(washingMachineCodesCommaSeparated);
        }
        
        return Result.Success;
    }
}