using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.DeleteInactivityType;

internal record DeleteInactivityTypeCommand(Guid Id) : ISettingsCommand<ErrorOr<Success>>;