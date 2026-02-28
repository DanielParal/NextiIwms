using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByKitId;

internal record GetBatchByKitIdQuery(Guid KitId) : IRequest<ErrorOr<Batch>>;