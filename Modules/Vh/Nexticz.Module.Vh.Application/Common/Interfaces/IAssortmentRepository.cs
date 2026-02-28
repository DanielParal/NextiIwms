using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Assortments.Common.Models;


namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IAssortmentRepository
{
    Task<Assortment?> GetAssortmentByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<AssortmentResponse?> GetAssortmentResponseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<FilteredResult> GetAssortmentsAsync(AssortmentsFilteringParams filteringParams,
        CancellationToken cancellationToken);
}