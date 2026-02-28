using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.UpdateEmailConfiguration;

internal record UpdateEmailConfigurationCommand(Guid Id, UpdateEmailConfigurationRequest UpdateRequest) : ISettingsCommand<ErrorOr<Success>>;