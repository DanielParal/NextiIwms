using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.CreatePartner;

internal record CreatePartnerCommand(string Code, string Name) : ISettingsCommand<ErrorOr<Partner>>;