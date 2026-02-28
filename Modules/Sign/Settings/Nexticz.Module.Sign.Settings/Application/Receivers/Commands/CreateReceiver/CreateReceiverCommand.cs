using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.CreateReceiver;

internal record CreateReceiverCommand(string Code, string PartnerCode, string Name) : ISettingsCommand<ErrorOr<Receiver>>;