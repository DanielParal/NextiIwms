using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers;

internal static class ReceiverContractFactory
{
    public static ReceiverContract Create(Receiver receiver)
    {
        return new ReceiverContract(receiver.Code, receiver.Name, receiver.PartnerCode);
    }
}