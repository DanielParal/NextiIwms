using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.DeletePartner;

internal record DeletePartnerCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;