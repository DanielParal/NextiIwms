
using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Queries.GetBatchesByWashingMachineCode;

internal record GetBatchesByWashingMachineCodeQuery(string WashingMachineCode) : IRequest<Batch[]>;