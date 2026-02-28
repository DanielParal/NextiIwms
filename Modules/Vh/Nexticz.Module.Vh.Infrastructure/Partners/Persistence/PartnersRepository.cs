using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.Partners.Common.Models;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.Partners.Persistence;

public class PartnersRepository(DataContext context) : IPartnersRepository
{
    public async Task<Partner?> GetPartnerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Partners
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PartnerResponse?> GetPartnerResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Partners
            .Where(x => x.Id == id)
            .Select(x => new PartnerResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Note = x.Note,
                Receipt = x.Receipt,
                ReceiptCoefficient = x.ReceiptCoefficient,
                Dispatch = x.Dispatch,
                DispatchCoefficient = x.DispatchCoefficient,
                Packaging = x.Packaging,
                PackagingCoefficient = x.PackagingCoefficient
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetPartnersAsync(PartnersFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Partners
            .Select(x => new PartnerResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Note = x.Note,
                Receipt = x.Receipt,
                ReceiptCoefficient = x.ReceiptCoefficient,
                Dispatch = x.Dispatch,
                DispatchCoefficient = x.DispatchCoefficient,
                Packaging = x.Packaging,
                PackagingCoefficient = x.PackagingCoefficient
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}