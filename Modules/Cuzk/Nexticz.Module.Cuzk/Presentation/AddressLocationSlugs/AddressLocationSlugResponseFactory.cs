using Nexticz.Module.Cuzk.Contracts.AddressLocationsSlugs;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocationSlugs;

internal static class AddressLocationSlugResponseFactory
{
    public static AddressLocationSlugResponse Create(AddressLocation addressLocation)
    {
        return new AddressLocationSlugResponse(
            addressLocation.Id,
            addressLocation.AdmCode,
            addressLocation.Slug
        );
    }
}