using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.UpdateWorker;

internal record UpdateWorkerCommand(Guid Id, string Name, int Pin, bool IsActive) : ISettingsCommand<ErrorOr<Updated>>;