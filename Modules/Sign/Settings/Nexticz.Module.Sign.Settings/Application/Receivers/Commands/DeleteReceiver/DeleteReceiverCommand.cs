using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.DeleteReceiver;

internal record DeleteReceiverCommand(string Code, string PartnerCode) : ISettingsCommand<ErrorOr<Deleted>>;