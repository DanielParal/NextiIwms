using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Application.Interfaces;
using Nexticz.Module.Mmo.Planning.Application.NotificationCollectors;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ChangeBatchKitsCount;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSingleBatch;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSisterBatches;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Orchestrators.SplitBatch;

internal class SplitBatchOrchestrator(
    ISender sender,
    IPlanningUnitOfWork unitOfWork,
    IPlanningNotificationCollector notificationCollector) : ISplitBatchOrchestrator
{
    public async Task<ErrorOr<Success>> OrchestrateSingleBatchAsync(Batch batch, string upperLineQueueCode, int countToChange, int kitsCountToCreate,
        CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        using (notificationCollector.DelayedNotifications())
        {
            var resultChangeKitsCount = 
                await sender.Send(new ChangeBatchKitsCountCommand(upperLineQueueCode, batch.Id, countToChange), cancellationToken);
            if (resultChangeKitsCount.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultChangeKitsCount.Errors;
            }
        
            var resultCreateKit = 
                await sender.Send(new CreateSingleBatchCommand(upperLineQueueCode, batch.KitCode, kitsCountToCreate, batch.PackagingCode), cancellationToken);
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

    public async Task<ErrorOr<Success>> OrchestrateSisterBatchesAsync(Batch batch, Batch sisterBatch, 
        string upperLineQueueCode, int countToChange, int kitsCountToCreate, CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        using (notificationCollector.DelayedNotifications())
        {
            var resultChangeKitsCount = 
                await sender.Send(new ChangeBatchKitsCountCommand(upperLineQueueCode, batch.Id, countToChange), cancellationToken);
            if (resultChangeKitsCount.IsError)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                notificationCollector.Clear();
                return resultChangeKitsCount.Errors;
            }
        
            var resultCreateKit = 
                await sender.Send(
                    new CreateSisterBatchesCommand(upperLineQueueCode, batch.KitCode, kitsCountToCreate, batch.PackagingCode, sisterBatch.PackagingCode), 
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