using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Assortments.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Vh.Infrastructure.Assortments.Persistence;

public class AssortmentsRepository(DataContext context) : IAssortmentRepository
{
    public async Task<Assortment?> GetAssortmentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Assortments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AssortmentResponse?> GetAssortmentResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Assortments
            .Where(x => x.Id == id)
            .Select(x => new AssortmentResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Receipt = x.Receipt,
                ReceiptCoefficient = x.ReceiptCoefficient,
                Dispatch = x.Dispatch,
                DispatchCoefficient = x.DispatchCoefficient,
                Packaging = x.Packaging,
                PackagingCoefficient = x.PackagingCoefficient
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> GetAssortmentsAsync(AssortmentsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Assortments
            .Select(x => new AssortmentResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
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