using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Workers.Queries;

public record GetWorkerResponseByPinQuery(int Pin) : IRequest<ErrorOr<WorkerResponse>>;