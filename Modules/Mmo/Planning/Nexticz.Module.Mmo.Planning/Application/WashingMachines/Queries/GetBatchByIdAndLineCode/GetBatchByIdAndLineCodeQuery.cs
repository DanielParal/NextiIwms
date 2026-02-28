using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetBatchByIdAndLineCode;

internal record GetBatchByIdAndLineCodeQuery(Guid BatchId, string LineCode) : IRequest<ErrorOr<Batch>>;