using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchById;

internal record GetBatchByIdQuery(Guid Id) : IRequest<ErrorOr<Batch>>;