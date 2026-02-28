using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachines;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachinesByCodes;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate.Events;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.CreatePackaging;

internal class CreatePackagingCommandHandler (
    ILogger<CreatePackagingCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<CreatePackagingCommand, ErrorOr<Packaging>>
{
    public async Task<ErrorOr<Packaging>> Handle(CreatePackagingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await IsValidAsync(request.CreatePackagingRequest, cancellationToken);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var packagingId = Guid.NewGuid();
        var uniqueCode = request.CreatePackagingRequest.DepositorCode.ToUpperInvariant() + request.CreatePackagingRequest.CustomerNumber.ToUpperInvariant();
        var washingMachineSpeeds = request.CreatePackagingRequest.WashingMachineSpeeds.Select(x => new WashingMachineSpeed(x.WashingMachineCode, (WashingMachineSpeedLevel)x.Speed)).ToArray();
        var packageCreatedEvent = new PackagingCreatedEvent(
            packagingId, uniqueCode, request.CreatePackagingRequest.PackagingTypeCode, request.CreatePackagingRequest.DepositorCode, 
            request.CreatePackagingRequest.PackagingCirculationCode,request.CreatePackagingRequest.CustomerNumber, 
            request.CreatePackagingRequest.Name, request.CreatePackagingRequest.MustBeWashed, 
            new Dimensions(request.CreatePackagingRequest.Dimensions.Depth, request.CreatePackagingRequest.Dimensions.Width, request.CreatePackagingRequest.Dimensions.Height), 
            request.CreatePackagingRequest.Weight, washingMachineSpeeds
            );
        
        unitOfWork.StartStream<PackagingCreatedEvent, Packaging>(packagingId, packageCreatedEvent);

        return new Packaging(
            uniqueCode, request.CreatePackagingRequest.PackagingTypeCode, request.CreatePackagingRequest.DepositorCode, 
            request.CreatePackagingRequest.PackagingCirculationCode, request.CreatePackagingRequest.CustomerNumber, 
            request.CreatePackagingRequest.Name, request.CreatePackagingRequest.MustBeWashed, request.CreatePackagingRequest.Weight, 
            new Dimensions(request.CreatePackagingRequest.Dimensions.Depth, request.CreatePackagingRequest.Dimensions.Width, request.CreatePackagingRequest.Dimensions.Height), 
            washingMachineSpeeds, packagingId
            );
    }

    private async Task<ErrorOr<Success>> IsValidAsync(CreatePackagingRequest request,
        CancellationToken cancellationToken)
    {
        var depositor = await sender.Send(new GetDepositorByCodeQuery(request.DepositorCode), cancellationToken);
        if (depositor.IsError)
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} does not exist. Nothing to create.", nameof(Depositor), request.DepositorCode);
            return PackagingErrors.ValidationDepositorDoesNotExist;
        }
        
        var packagingType = await sender.Send(new GetPackagingTypeByCodeQuery(request.PackagingTypeCode), cancellationToken);
        if (packagingType.IsError)
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} does not exist. Nothing to create.", nameof(PackagingType), request.PackagingTypeCode);
            return PackagingErrors.ValidationPackagingTypeDoesNotExist;
        }
        
        var packagingCirculation = await sender.Send(new GetPackagingCirculationByCodeQuery(request.PackagingCirculationCode), cancellationToken);
        if (packagingCirculation.IsError)
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} does not exist. Nothing to create.", nameof(PackagingCirculation), request.PackagingCirculationCode);
            return PackagingErrors.ValidationPackagingCirculationDoesNotExist;
        }

        var uniqueCode = depositor.Value.Code + request.CustomerNumber.ToUpperInvariant();
        var existingPackaging = await sender.Send(new GetPackagingByCodeQuery(uniqueCode), cancellationToken);
        if (existingPackaging.HasValue())
        {
            logger.LogInformation("Combination of depositor code: {DepositorCode} and customer number: {CustomerNumber} already exists. Nothing to create.", 
                request.DepositorCode, request.CustomerNumber);
            return PackagingErrors.ValidationCombinationDepositorAndNumberExists;
        }
        
        var codesFromRequest = request.WashingMachineSpeeds.Select(x => x.WashingMachineCode).ToArray();
        var washingMachinesFromRequest = await sender.Send(new GetWashingMachinesByCodesQuery(codesFromRequest), cancellationToken);
        var washingMachines = await sender.Send(new GetWashingMachinesQuery(new BaseFilteringParams()), cancellationToken);
        if (washingMachinesFromRequest.Count != washingMachines.Data.Count || codesFromRequest.Length > washingMachines.Data.Count)
        {
            var washingMachineCodesCommaSeparated = string.Join(", ", washingMachines.Data.Select(x => x.Code));
            var specifiedCodesCommaSeparated = string.Join(", ", codesFromRequest);
            logger.LogInformation("{ObjectName} with customer number: {CustomerNumber} does not provide the correct number of speeds for machine machines. " +
                                  "Provided codes: {ProvidedCodes}. " +
                                  "You need to specify speeds for machine codes: {WashingMachineCodes}. Nothing to create.", 
                nameof(Packaging), request.CustomerNumber, specifiedCodesCommaSeparated, washingMachineCodesCommaSeparated);
            return PackagingErrors.ValidationPackagingSpeedsCountMismatch(washingMachineCodesCommaSeparated);
        }
        
        return Result.Success;
    }
}

