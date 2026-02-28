using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSingleBatch;

internal record CreateSingleBatchCommand(
    string LineQueueCode,
    string KitCode,
    int KitsCount, 
    string PackagingCode) : IPlanningCommand<ErrorOr<Batch>>;