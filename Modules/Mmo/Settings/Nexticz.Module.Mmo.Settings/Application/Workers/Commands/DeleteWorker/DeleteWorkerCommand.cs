using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Workers.Commands.DeleteWorker;

internal record DeleteWorkerCommand(Guid Id) : ISettingsCommand<ErrorOr<Deleted>>;