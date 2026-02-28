using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.UpdatePartner;

internal record UpdatePartnerCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Updated>>;