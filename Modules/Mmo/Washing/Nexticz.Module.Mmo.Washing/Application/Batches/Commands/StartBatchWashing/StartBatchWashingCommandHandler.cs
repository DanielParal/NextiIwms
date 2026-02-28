using ErrorOr;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;
using Nexticz.Module.Mmo.Washing.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSpeedAggregate;
using BatchContract = Nexticz.Module.Mmo.Planning.Contracts.WashingMachines.BatchContract;
using WashingMachineResponse = Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.WashingMachineResponse;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.StartBatchWashing;

internal class StartBatchWashingCommandHandler(
    ISender sender,
    ILogger<StartBatchWashingCommandHandler> logger,
    IWashingUnitOfWork unitOfWork) 
    : IRequestHandler<StartBatchWashingCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(StartBatchWashingCommand request, CancellationToken cancellationToken)
    {
        var kitSapDefinitionName = await GetKitSapDefinitionNameAsync(request.Batch.KitSapDefinitionCode, request.Batch.Id, cancellationToken);
        var sapBarcode = await GetSapBarcodeAsync(request.Batch, cancellationToken);
        var specialInformation = await GetSpecialInformationAsync(request.Batch.KitCode, cancellationToken);
        var washingMachine = await GetWashingMachineAsync(request.WashingMachineCode, request.Batch.Id, cancellationToken);
        if (washingMachine.IsError)
            return washingMachine.Errors;
        var packagingSpeed = await GetOptimalPackagingSpeedAsync(request.Batch.PackagingCode, washingMachine.Value, cancellationToken);
        
        var batch = new Batch(
            request.Batch.SisterBatchId,
            request.WashingMachineCode.ToUpperInvariant(),
            washingMachine.Value.Length,
            request.LineCode.ToUpperInvariant(),
            request.Batch.KitCode,
            request.Batch.KitNumber,
            request.Batch.KitsCount,
            request.Batch.KitSapDefinitionCode,
            kitSapDefinitionName,
            sapBarcode,
            request.Batch.PackagingCode,
            packagingSpeed.OptimalPackagingSpeed,
            packagingSpeed.OptimalPackagingSpeedLevel,
            request.RequestedPackagingSpeed, // This should be replaced in the future with actual washing machine speed - currently we don't have the info about real time speed of the machine
            request.RequestedPackagingSpeedLevel,
            request.Batch.PackagingHeight,
            request.Batch.DefiningPackagingCode,
            request.Batch.OptimalKitDuration,
            request.DateStarted,
            request.PreviousBatchLastKitEndDate,
            request.ShouldFirstKitStartAfterPreviousBatchLastKitEndDate,
            [],
            [],
            specialInformation,
            request.Batch.Id);
        
        var createBatchEvent = 
            new BatchCreatedEvent(batch.Id, batch.SisterBatchId, batch.WashingMachineCode, batch.WashingMachineLength, 
                batch.LineCode, batch.KitCode, batch.KitNumber, batch.PlannedKitsCount,
                batch.KitSapDefinitionCode, batch.KitSapDefinitionName, batch.SapBarcode, batch.PackagingCode, batch.OptimalPackagingSpeedOnCurrentMachine,
                batch.OptimalPackagingSpeedOnCurrentMachineLevel, batch.PackagingSpeed, batch.PackagingSpeedLevel, batch.PackagingHeight, 
                batch.DefiningPackagingCode, batch.OptimalKitDuration, batch.DateActivated, batch.PreviousBatchLastKitEndDate, 
                batch.ShouldFirstKitStartAfterPreviousBatchLastKitEndDate, specialInformation);
        
        unitOfWork.StartStream<BatchCreatedEvent, Batch>(batch.Id, createBatchEvent); 
        
        logger.LogInformation("Washing - Batch with id: {BatchId} started at DateTimeOffset: {DateActivated}.",
            batch.Id, batch.DateActivated);

        return Result.Success;
    }

    private async Task<string> GetKitSapDefinitionNameAsync(string kitSapDefinitionCode, Guid batchId,
        CancellationToken cancellationToken)
    {
        var kitSapDefinition = 
            await sender.Send(new GetKitSapDefinitionResponseByCodeQuery(kitSapDefinitionCode), cancellationToken);

        if (kitSapDefinition.IsError)
        {
            logger.LogWarning("Washing - Cannot find kit sap definition with code: {KitSapDefinitionCode} when starting batch. BatchId: {BatchId}.", 
                kitSapDefinitionCode, batchId);
            return string.Empty;
        }

        return kitSapDefinition.Value.Name;
    }

    private record ObjectForBarcodeComposer(string KitNumber, string ManufactureCode);
    
    private async Task<string> GetSapBarcodeAsync(BatchContract batch,
        CancellationToken cancellationToken)
    {
        var depositor = 
            await sender.Send(new GetDepositorResponseByCodeQuery(batch.DepositorCode), cancellationToken);

        if (depositor.IsError)
        {
            logger.LogWarning("Washing - Cannot find depositor with code: {DepositorCode} when starting batch. BatchId: {BatchId}.", 
                batch.DepositorCode, batch.Id);
            return string.Empty;
        }
        
        if (string.IsNullOrWhiteSpace(depositor.Value.BarcodeTemplate))
            return string.Empty;

        var manufactureCode = await GetManufactureCodeAsync(batch.KitCode, batch.Id, cancellationToken);
        
        return SapBarcodeComposer.Compose(new ObjectForBarcodeComposer(batch.KitNumber, manufactureCode), depositor.Value.BarcodeTemplate);
    }
    
    private async Task<string> GetManufactureCodeAsync(string kitCode, Guid batchId, CancellationToken cancellationToken)
    {
        var kitResponse = 
            await sender.Send(new GetKitResponseByCodeQuery(kitCode), cancellationToken);

        if (kitResponse.IsError)
        {
            logger.LogWarning("MMO - Washing - Cannot find kit with code: {KitSapDefinitionCode} when starting batch. BatchId: {BatchId}.", 
                kitCode, batchId);
            return string.Empty;
        }

        return kitResponse.Value.ManufactureCode;
    }
    
    private async Task<SpecialInformation?> GetSpecialInformationAsync(string kitCode,
        CancellationToken cancellationToken)
    {
        var specialInformationFromSettings = await sender.Send(new GetCurrentKitSpecialInformationResponseByKitCodeQuery(kitCode), cancellationToken);

        if (specialInformationFromSettings is null)
            return null;
        
        return new SpecialInformation(
            specialInformationFromSettings.Title,
            specialInformationFromSettings.Description,
            specialInformationFromSettings.HasFile,
            [],
            specialInformationFromSettings.Id);
    }
    
    private async Task<ErrorOr<WashingMachineResponse>> GetWashingMachineAsync(string washingMachineCode, Guid batchId,
        CancellationToken cancellationToken)
    {
        var washingMachineFromSettings = await sender.Send(new GetWashingMachineResponseByCodeQuery(washingMachineCode), cancellationToken);

        if (washingMachineFromSettings.IsError)
        {
            logger.LogError("Washing - Cannot get washing machine speed because washing machine with code: {WashingMachineCode} does not exist. BatchId: {BatchId}.", 
                washingMachineCode, batchId);
            return BatchErrors.ValidationWashingMachineDoesNotExist;
        }
        
        return washingMachineFromSettings.Value;
    }

    private async Task<(int OptimalPackagingSpeed, SpeedLevel OptimalPackagingSpeedLevel)> GetOptimalPackagingSpeedAsync(string packagingCode, WashingMachineResponse washingMachineResponse, CancellationToken cancellationToken)
    {
        var packagingFromSettings = await sender.Send(new GetPackagingResponseByCodeQuery(packagingCode), cancellationToken);

        if (packagingFromSettings.IsError)
        {
            logger.LogError("MMO - Washing - Cannot get packaging speed for packaging with code: {PackagingCode}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}.", 
                packagingCode, packagingFromSettings.FirstError.Code, packagingFromSettings.FirstError.Description);
            return (0, SpeedLevel.NotSet);
        }
        
        return GetRequestedWashingMachineSpeed(packagingFromSettings.Value, washingMachineResponse);
    }
    
    private (int Speed, SpeedLevel SpeedLevel) GetRequestedWashingMachineSpeed(PackagingResponse packagingResponse, WashingMachineResponse washingMachineResponse)
    {
        var packagingSpeedLevel = packagingResponse.WashingMachineSpeeds.FirstOrDefault(x => x.WashingMachineCode == washingMachineResponse.Code)?.Speed;
        if (packagingSpeedLevel is null)
        {
            logger.LogError(
                "MMO - Washing - Invalid speed level: speed for washing machine with code: {WashingMachineCode} does not exist. Packaging code: {PackagingCode}.",
                washingMachineResponse.Code, packagingResponse.Code);
            return (0, SpeedLevel.NotSet);
        } 
        
        var propertyName = packagingSpeedLevel.Value.ToString();
        
        var speedProperty = washingMachineResponse.GetType().GetProperty(propertyName);
        if (speedProperty == null) 
        {
            logger.LogError(
                "MMO - Washing - Invalid speed level: {SpeedLevel} requested for washing machine with code: {Code}.",
                packagingSpeedLevel.Value, washingMachineResponse.Code);
            return (0, SpeedLevel.NotSet);
        }
        
        if (speedProperty.GetValue(washingMachineResponse) is not int speedValue)
        {
            logger.LogError(
                "MMO - Washing - Washing machine with code: {Code} does not contain a valid speed value for level {SpeedLevel}.",
                washingMachineResponse.Code, packagingSpeedLevel.Value);
            return (0, SpeedLevel.NotSet);
        }
        
        return (speedValue, (SpeedLevel)packagingSpeedLevel.Value);
    }
}