using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners;

internal static class PartnerContractFactory
{
    public static PartnerContract Create(Partner partner)
    {
        return new PartnerContract(partner.Code, partner.Name);
    }
}