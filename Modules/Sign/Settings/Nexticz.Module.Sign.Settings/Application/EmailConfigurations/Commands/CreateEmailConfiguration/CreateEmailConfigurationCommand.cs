using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.CreateEmailConfiguration;

internal record CreateEmailConfigurationCommand(CreateEmailConfigurationRequest CreateRequest) : ISettingsCommand<ErrorOr<EmailConfiguration>>;