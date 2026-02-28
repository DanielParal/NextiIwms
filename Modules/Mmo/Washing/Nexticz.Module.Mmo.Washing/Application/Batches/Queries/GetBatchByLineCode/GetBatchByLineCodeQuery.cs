using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchByLineCode;

internal record GetBatchByLineCodeQuery(string LineCode) : IRequest<Batch?>;