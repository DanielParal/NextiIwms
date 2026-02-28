using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Receivers;

internal static class ReceiverResponseFactory
{
    public static ReceiverResponse Create(Receiver receiver)
    {
        return new ReceiverResponse(receiver.Id, receiver.Code, receiver.Name, receiver.PartnerCode);
    }
}