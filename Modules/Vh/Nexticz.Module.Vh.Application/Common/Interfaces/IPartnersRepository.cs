using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Partners.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IPartnersRepository
{
    Task<Partner?> GetPartnerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PartnerResponse?> GetPartnerResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetPartnersAsync(PartnersFilteringParams filteringParams,
        CancellationToken cancellationToken);
}