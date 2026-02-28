using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.UpdateReceiver;

internal record UpdateReceiverCommand(string Code, string PartnerCode, string Name) : ISettingsCommand<ErrorOr<Updated>>;