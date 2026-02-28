using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.CreateWorker;

internal record CreateWorkerCommand(string Name, int Pin, bool IsActive) : ISettingsCommand<ErrorOr<Worker>>;