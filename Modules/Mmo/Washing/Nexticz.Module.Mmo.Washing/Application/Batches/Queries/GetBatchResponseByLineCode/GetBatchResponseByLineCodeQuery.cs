using MediatR;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchResponseByLineCode;

internal record GetBatchResponseByLineCodeQuery(string LineCode) : IRequest<BatchResponse?>;