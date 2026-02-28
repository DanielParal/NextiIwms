using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Queries.GetWorkerByPin;

internal record GetWorkerByPinQuery(int Pin) : IRequest<ErrorOr<Worker>>;