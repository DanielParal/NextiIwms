using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSisterBatches;

internal record CreateSisterBatchesCommand(
    string LineQueueCode,
    string KitCode,
    int KitsCount, 
    string PackagingCode,
    string SisterPackagingCode) : IPlanningCommand<ErrorOr<(Batch Batch, Batch SisterBatch)>>;