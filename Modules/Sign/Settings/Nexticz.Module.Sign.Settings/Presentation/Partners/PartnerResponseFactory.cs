using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Partners;

internal static class PartnerResponseFactory
{
    public static PartnerResponse Create(Partner partner)
    {
        return new PartnerResponse(partner.Id, partner.Code, partner.Name);
    }
}