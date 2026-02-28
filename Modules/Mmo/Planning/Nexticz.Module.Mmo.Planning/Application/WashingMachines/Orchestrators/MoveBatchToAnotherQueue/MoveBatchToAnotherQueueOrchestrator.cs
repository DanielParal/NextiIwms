using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSingleBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSisterBatches;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.RemoveBatch;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.MoveBatchToAnotherQueue;

internal class MoveBatchToAnotherQueueOrchestrator(
    ISender sender,
    IPlanningUnitOfWork unitOfWork,
    IPlanningNotificationCollector notificationCollector) : IMoveBatchToAnotherQueueOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateSingleBatchAsync(Batch batch, string upperCurrentLineQueueCode, string newUpperLineQueueCode,
        CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        using (notificationCollector.DelayedNotifications())
        {
            var resultChangeKitsCount = 
                await sender.Send(new RemoveBatchCommand(upperCurrentLineQueueCode, batch.Id), cancellationToken);
            if (resultChangeKitsCount.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultChangeKitsCount.Errors;
            }
        
            var resultCreateKit = 
                await sender.Send(new CreateSingleBatchCommand(newUpperLineQueueCode, batch.KitCode, batch.KitsCount, batch.PackagingCode), cancellationToken);
            if (resultCreateKit.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultCreateKit.Errors;
            }
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        await notificationCollector.PublishNotificationsAsync(cancellationToken);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> OrchestrateSisterBatchesAsync(Batch batch, Batch sisterBatch, string upperCurrentLineQueueCode,
        string newUpperLineQueueCode, CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        using (notificationCollector.DelayedNotifications())
        {
            var resultChangeKitsCount = 
                await sender.Send(new RemoveBatchCommand(upperCurrentLineQueueCode, batch.Id), cancellationToken);
            if (resultChangeKitsCount.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultChangeKitsCount.Errors;
            }
        
            var resultCreateKit = 
                await sender.Send(
                    new CreateSisterBatchesCommand(newUpperLineQueueCode, batch.KitCode, batch.KitsCount, batch.PackagingCode, sisterBatch.PackagingCode), 
                    cancellationToken);
            if (resultCreateKit.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultCreateKit.Errors;
            }
        }
        
        await unitOfWork.CommitTransactionAsync(cancellationToken);
        await notificationCollector.PublishNotificationsAsync(cancellationToken);
        
        return Result.Success;
    }
}