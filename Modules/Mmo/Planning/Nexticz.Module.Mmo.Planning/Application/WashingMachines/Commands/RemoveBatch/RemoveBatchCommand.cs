using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.RemoveBatch;

internal record RemoveBatchCommand(string LineQueueCode, Guid BatchId) : IPlanningCommand<ErrorOr<Deleted>>;