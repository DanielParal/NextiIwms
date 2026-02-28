using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate.Events;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSingleBatch;

internal class CreateSingleBatchCommandHandler(
    ICreateBatchesBaseCommandHandler baseCommandHandler,
    ILogger<CreateSingleBatchCommandHandler> logger,
    IPlanningUnitOfWork unitOfWork)
    : IRequestHandler<CreateSingleBatchCommand, ErrorOr<Batch>>
{
    public async Task<ErrorOr<Batch>> Handle(CreateSingleBatchCommand request, CancellationToken cancellationToken)
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
        
        // 4. get washing machine
        var filteredWashingMachineResponses = await baseCommandHandler.GetFilteredWashingMachineResponsesByPackagingCodeAsync(request.PackagingCode, null, cancellationToken);
        var washingMachine = await baseCommandHandler.GetWashingMachineByLineQueueCodeAsync(
            upperLineQueueCode, filteredWashingMachineResponses, cancellationToken);
        if (washingMachine.IsError)
            return washingMachine.Errors;
        
        // 5. get mmo constant
        var spaceBetweenPackagingsConstant = await baseCommandHandler.GetSpaceBetweenPackagingsConstantAsync(cancellationToken);
        
        // 6. calculate optimal washing time
        var calculatedOptimalTime = baseCommandHandler.CalculateOptimalWashingTime(
            kitFromSettings.Value, 
            packagingFromSettings.Value, 
            filteredWashingMachineResponses.First(x => x.Code == washingMachine.Value.Code), 
            spaceBetweenPackagingsConstant);
        
        if (calculatedOptimalTime.IsError)
            return calculatedOptimalTime.Errors;
        
        // 8. create event
        var batch = new Batch(
            null, kitFromSettings.Value.DepositorCode, kitFromSettings.Value.Code, kitFromSettings.Value.KitNumber, request.KitsCount, 
            packagingFromSettings.Value.Code, packagingFromSettings.Value.Dimensions.Height, kitFromSettings.Value.DefiningPackagingCode,
            kitFromSettings.Value.KitSapDefinitionCode, calculatedOptimalTime.Value);
        var addBatchResult = washingMachine.Value.AddBatch(batch, upperLineQueueCode);

        if (addBatchResult.IsError)
        {
            logger.LogWarning(
                "Cannot add batch to washing machine with code: {Code}. " +
                "Error code: {ErrorCode}, error description: {ErrorDescription}." +
                "KitCode: {KitCode}, PackagingCode: {PackagingCode}, KitsCount: {KitsCount}, OptimalKitDuration: {OptimalKitDuration}.", 
                washingMachine.Value.Code, addBatchResult.FirstError.Code, addBatchResult.FirstError.Description,
                batch.KitCode, batch.PackagingCode, batch.KitsCount, batch.OptimalKitDuration);
            return addBatchResult.Errors;
        }

        var batchAddedEvent = new BatchCreatedEvent(
            batch.Id,
            washingMachine.Value.Code, 
            upperLineQueueCode, 
            batch.DepositorCode,
            batch.KitCode, 
            batch.KitNumber,
            batch.KitsCount, 
            batch.PackagingCode, 
            batch.PackagingHeight,
            batch.DefiningPackagingCode,
            batch.KitSapDefinitionCode,
            batch.OptimalKitDuration);
        
        unitOfWork.AppendEvent(washingMachine.Value.Id, batchAddedEvent);

        return batch;
    }
}