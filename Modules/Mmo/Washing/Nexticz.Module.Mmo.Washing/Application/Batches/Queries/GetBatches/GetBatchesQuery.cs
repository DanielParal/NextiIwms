using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatches;

internal record GetBatchesQuery() : IRequest<Batch[]>;