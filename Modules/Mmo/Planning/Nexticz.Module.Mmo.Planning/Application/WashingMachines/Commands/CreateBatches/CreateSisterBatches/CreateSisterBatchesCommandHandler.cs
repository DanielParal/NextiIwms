using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSisterBatches;

internal class CreateSisterBatchesCommandHandler(
    ISender sender,
    ICreateBatchesBaseCommandHandler baseCommandHandler,
    ILogger<CreateSisterBatchesCommandHandler> logger,
    IPlanningUnitOfWork unitOfWork) 
    : IRequestHandler<CreateSisterBatchesCommand, ErrorOr<(Batch Batch, Batch SisterBatch)>>
{
    public async Task<ErrorOr<(Batch Batch, Batch SisterBatch)>> Handle(CreateSisterBatchesCommand request, CancellationToken cancellationToken)
    {
        var upperLineQueueCode = request.LineQueueCode.ToUpperInvariant();
        // 1. get kit response
        var kitFromSettings = await baseCommandHandler.GetKitResponseByCodeAsync(request.KitCode, cancellationToken);
        if (kitFromSettings.IsError)
            return kitFromSettings.Errors;

        // 2. get packaging response
        var packagingFromSettings = await baseCommandHandler.GetPackagingResponseByCodeAsync(request.PackagingCode, cancellationToken);
        if (packagingFromSettings.IsError)
            return packagingFromSettings.Errors;
        
        // 3. verify if packaging is in list of kits packagings
        var isPackagingInKit = baseCommandHandler.IsPackagingInKit(packagingFromSettings.Value, kitFromSettings.Value);
        if (isPackagingInKit.IsError)
            return isPackagingInKit.Errors;
        
        // 4. get sister packaging response
        var sisterPackagingFromSettings = 
            await GetSisterPackagingResponseByCodeAsync(request.SisterPackagingCode, kitFromSettings.Value, cancellationToken);
        if (sisterPackagingFromSettings.IsError)
            return sisterPackagingFromSettings.Errors;
        
        // 5. get washing machine
        var filteredWashingMachineResponses = 
            await baseCommandHandler.GetFilteredWashingMachineResponsesByPackagingCodeAsync(
                request.PackagingCode, request.SisterPackagingCode, cancellationToken);
        var washingMachine = await baseCommandHandler.GetWashingMachineByLineQueueCodeAsync(
            upperLineQueueCode, filteredWashingMachineResponses, cancellationToken);
        if (washingMachine.IsError)
            return washingMachine.Errors;
        
        // 6. get mmo constant
        var spaceBetweenPackagingsConstant = await baseCommandHandler.GetSpaceBetweenPackagingsConstantAsync(cancellationToken);
        
        // 7. calculate optimal washing time
        var calculatedOptimalTime = 
            baseCommandHandler.CalculateOptimalWashingTime(
            kitFromSettings.Value, 
            packagingFromSettings.Value, 
            filteredWashingMachineResponses.First(x => x.Code == washingMachine.Value.Code), 
            spaceBetweenPackagingsConstant);
        
        if (calculatedOptimalTime.IsError)
            return calculatedOptimalTime.Errors;
        
        var calculatedSisterOptimalTime = 
            baseCommandHandler.CalculateOptimalWashingTime(
                kitFromSettings.Value, 
                sisterPackagingFromSettings.Value, 
                filteredWashingMachineResponses.First(x => x.Code == washingMachine.Value.Code), 
                spaceBetweenPackagingsConstant);
        
        if (calculatedSisterOptimalTime.IsError)
            return calculatedSisterOptimalTime.Errors;
        
        // 8. create event
        var batchId = Guid.NewGuid();
        var sisterBatchId = Guid.NewGuid();
        var batch = new Batch(
            sisterBatchId, kitFromSettings.Value.DepositorCode, kitFromSettings.Value.Code, kitFromSettings.Value.KitNumber, request.KitsCount, 
            packagingFromSettings.Value.Code, packagingFromSettings.Value.Dimensions.Height, kitFromSettings.Value.DefiningPackagingCode, 
            kitFromSettings.Value.KitSapDefinitionCode, calculatedOptimalTime.Value, id: batchId);

        var sisterBatch = new Batch(
            batchId, kitFromSettings.Value.DepositorCode, kitFromSettings.Value.Code, kitFromSettings.Value.KitNumber, request.KitsCount,
            sisterPackagingFromSettings.Value.Code, sisterPackagingFromSettings.Value.Dimensions.Height, kitFromSettings.Value.DefiningPackagingCode, 
            kitFromSettings.Value.KitSapDefinitionCode, calculatedSisterOptimalTime.Value, id: sisterBatchId);
        
        var addBatchesResponse = washingMachine.Value.AddSisterBatches(batch, sisterBatch);
        
        if (addBatchesResponse.IsError)
        {
            logger.LogWarning(
                "Cannot add batch to washing machine with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "KitCode: {KitCode}, PackagingCode: {PackagingCode}, " +
                "SisterPackagingCode: {SisterPackagingCode}, KitsCount: {KitsCount}.", 
                washingMachine.Value.Code, addBatchesResponse.FirstError.Code, addBatchesResponse.FirstError.Description,
                batch.KitCode, batch.PackagingCode, sisterBatch.PackagingCode, batch.KitsCount);
            return addBatchesResponse.Errors;
        }
        
        var sisterBatchCreatedEvent = new SisterBatchCreatedEvent(
            batch.Id, 
            sisterBatch.Id,
            washingMachine.Value.Code, 
            washingMachine.Value.LineQueues[0].WashingMachineLineCode, 
            washingMachine.Value.LineQueues[1].WashingMachineLineCode, 
            batch.DepositorCode,
            batch.KitCode, 
            batch.KitNumber,
            batch.KitSapDefinitionCode,
            batch.KitsCount, 
            batch.PackagingCode,
            batch.PackagingHeight,
            sisterBatch.PackagingCode, 
            sisterBatch.PackagingHeight,
            batch.DefiningPackagingCode,
            batch.OptimalKitDuration,
            sisterBatch.OptimalKitDuration);
        
        unitOfWork.AppendEvent(washingMachine.Value.Id, sisterBatchCreatedEvent);
        
        return (batch, sisterBatch);
    }
    
    private async Task<ErrorOr<PackagingResponse>> GetSisterPackagingResponseByCodeAsync(string packagingCode, KitResponse kitFromSettings, 
        CancellationToken cancellationToken)
    {
        var sisterPackaging = await sender.Send(new GetPackagingResponseByCodeQuery(packagingCode), cancellationToken);
            
        if (sisterPackaging.IsError)
        {
            logger.LogWarning("Sister object {ObjectName} with code: {Code} does not exist. We cannot add batch to queue.",
                nameof(PackagingResponse), packagingCode);
            return WashingMachineErrors.ValidationPackagingWithCodeDoesNotExistInSettings;
        }
            
        if (kitFromSettings.PackagingQuantities
            .All(x => x.PackagingCode != sisterPackaging.Value.Code))
        {
            logger.LogWarning("Sister packaging with code: {Code} is not in list of packagings for kit with code: {KitCode}. We cannot add batch to queue.",
                packagingCode, kitFromSettings.Code);
            return WashingMachineErrors.ValidationPackagingIsNotListedInKitPackagings;
        }

        return sisterPackaging.Value;
    }
}