using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.DeleteEmailConfiguration;

internal record DeleteEmailConfigurationCommand(Guid Id) : ISettingsCommand<ErrorOr<Success>>;