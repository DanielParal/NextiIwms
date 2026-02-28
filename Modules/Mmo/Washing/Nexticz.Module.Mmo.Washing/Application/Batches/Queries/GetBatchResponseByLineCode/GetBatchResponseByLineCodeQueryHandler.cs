using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;
using Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByLineCode;
using Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Queries.GetLastWorkerByLineCode;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSosByCode;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchResponseByLineCode;

internal class GetBatchResponseByLineCodeQueryHandler(
    ISender sender,
    ILogger<GetBatchResponseByLineCodeQueryHandler> logger
    ) : IRequestHandler<GetBatchResponseByLineCodeQuery, BatchResponse?>
{
    public async Task<BatchResponse?> Handle(GetBatchResponseByLineCodeQuery request, CancellationToken cancellationToken)
    {
        var batch = 
            await sender.Send(
                new GetBatchByLineCodeQuery(request.LineCode), cancellationToken);

        if (batch is null)
            return null;
        
        var worker = await GetWorkerAsync(request.LineCode, cancellationToken);
        var logoutInfo = GetLogoutInfo(worker?.Name);
        var completionInfo = await GetInfoAboutCompletionAsync(batch, worker?.Name, cancellationToken);
        var isSpecialInformationConfirmationNeeded = worker?.Name is not null && batch.IsSpecialInformationConfirmationNeededByWorker(worker.Name);
        var isHelpNeeded = await IsHelpNeededAsync(batch.WashingMachineCode, cancellationToken);
        
        return BatchResponseFactory.Create(
            batch, 
            isSpecialInformationConfirmationNeeded, 
            logoutInfo.ShouldBeLoggedOut, 
            logoutInfo.ReasonForLogout, 
            completionInfo.CanCompleteKit, 
            completionInfo.ReasonWhyKitCannotBeCompleted,
            isHelpNeeded);
    }
    
    private async Task<(bool CanCompleteKit, string? ReasonWhyKitCannotBeCompleted)> GetInfoAboutCompletionAsync(Batch batch, string? workerName, CancellationToken cancellationToken)
    {
        if (workerName is null)
            return (false, BatchTranslations.NoUserAtLineLogoutNeeded.TranslationValue);
        
        if (batch.IsSpecialInformationConfirmationNeededByWorker(workerName))
            return (false, BatchTranslations.SpecialInformationConfirmationNeededFromUser.TranslationValue);

        if (batch.SisterBatchId is null)
            return (true, null);
        
        var sisterBatch = await sender.Send(
            new GetBatchByIdQuery((Guid)batch.SisterBatchId), cancellationToken);
        
        if (sisterBatch.IsError)
            return (false, BatchTranslations.SisterBatchDoesNotExist.TranslationValue);
        
        var sisterWorker = await GetWorkerAsync(sisterBatch.Value.LineCode, cancellationToken);
        if (sisterWorker is null)
            return (false, BatchTranslations.NoUserAtAnotherLineWeNeedToWait.TranslationValue);
        
        if (sisterBatch.Value.IsSpecialInformationConfirmationNeededByWorker(sisterWorker.Name))
            return (false, BatchTranslations.SpecialInformationConfirmationNeededFromUserFromOtherLine.TranslationValue);
        
        return (true, null);
    }

    private static (bool ShouldBeLoggedOut, string? ReasonForLogout) GetLogoutInfo(string? workerName)
    {
        if (workerName is null)
            return (true, BatchTranslations.NoUserAtLineLogoutNeeded.TranslationValue);
        
        return (false, null);
    }
    
    private async Task<Worker?> GetWorkerAsync(string lineCode, CancellationToken cancellationToken)
    {
        var lastEnteredWorkerOnLine = await sender.Send(new GetLastWorkerByLineCodeQuery(lineCode), cancellationToken);
        if (lastEnteredWorkerOnLine.IsError || lastEnteredWorkerOnLine.Value.Worker is null)
        {
            logger.LogWarning("Washing - there is nobody at the line. We need to logout current user from frontend. LineCode: {LineCode}.", lineCode);
            return null;
        }

        return lastEnteredWorkerOnLine.Value.Worker!;
    }

    private async Task<bool> IsHelpNeededAsync(string washingMachineCode, CancellationToken cancellationToken)
    {
        var washingMachineSos = await sender.Send(new GetWashingMachineSosByCodeQuery(washingMachineCode), cancellationToken);
        if (washingMachineSos is null)
            return false;
        
        return washingMachineSos.IsHelpNeeded;
    }
}