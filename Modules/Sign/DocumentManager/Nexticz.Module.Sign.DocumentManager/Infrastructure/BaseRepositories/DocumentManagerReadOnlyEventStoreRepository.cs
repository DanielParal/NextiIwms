using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Partners.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Receivers.Queries;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.BaseRepositories;

internal class DocumentManagerReadOnlyEventStoreRepository(
    IDocumentManagerDocumentSessionProvider documentSessionProvider,
    ISender sender) 
    : Module.Sign.SharedKernel.DataAccess.ReadOnlyEventStoreRepository(documentSessionProvider), IDocumentManagerReadOnlyEventStoreRepository
{
    private readonly IQuerySession _session = documentSessionProvider.GetSession();
    
    public async Task<FilteredResult<UnsignedLoadingDocumentView>> GetUnsignedLoadingDocumentsForUserAsync(
        BaseFilteringParams filteringParams,
        string[] depositorCodes,
        CancellationToken cancellationToken)
    {
        var query = _session.Query<UnsignedLoadingDocumentView>()
            .Where(x => x.DepositorCode.IsOneOf(depositorCodes));

        return await GetFilteredAsync(query, filteringParams, cancellationToken);
    }

    public async Task<FilteredResult<SignedLoadingDocumentView>> GetSignedLoadingDocumentsForUserAsync(
        BaseFilteringParams filteringParams,
        string[] depositorCodes, 
        string? partnersOrderNumber,
        string? rznoCode,
        string? combinedRznoCode,
        string? partnerName,
        string? receiverName,
        string? fullTextFilter,
        CancellationToken cancellationToken)
    {
        var query = _session.Query<SignedLoadingDocumentView>()
            .Where(x => x.DepositorCode.IsOneOf(depositorCodes));

        if (!string.IsNullOrWhiteSpace(partnersOrderNumber))
            query = FilterPartnersOrderNumber(query, partnersOrderNumber);
        
        if (!string.IsNullOrWhiteSpace(rznoCode))
            query = FilterRznoCode(query, rznoCode);
        
        if (!string.IsNullOrWhiteSpace(combinedRznoCode))
            query = FilterCombinedRznoCode(query, combinedRznoCode);
        
        if (!string.IsNullOrWhiteSpace(partnerName))
            query = await FilterPartnerNameAsync(query, partnerName, cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(receiverName))
            query = await FilterReceiverNameAsync(query, receiverName, cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(fullTextFilter))
            query = await FilterFullTextAsync(query, fullTextFilter, cancellationToken);
        
        return await GetFilteredAsync(query, filteringParams, cancellationToken);
    }
    
    private async Task<IQueryable<SignedLoadingDocumentView>> FilterReceiverNameAsync(IQueryable<SignedLoadingDocumentView> query, string receiverName, CancellationToken cancellationToken)
    {
        var receivers = await sender.Send(new GetReceiverContractsByNameQuery(receiverName), cancellationToken);
        var receiverCodes = receivers.Select(x => x.Code).ToArray();
            
        return query.Where(
            x => 
                x.DeliveryDocuments.Any(
                    y => y.OperationalUnitCode.IsOneOf(receiverCodes)));
    }
    private async Task<IQueryable<SignedLoadingDocumentView>> FilterPartnerNameAsync(IQueryable<SignedLoadingDocumentView> query, string partnerName, CancellationToken cancellationToken)
    {
        var partners = await sender.Send(new GetPartnerContractsByNameQuery(partnerName), cancellationToken);
        var partnerCodes = partners.Select(x => x.Code).ToArray();
            
        return query.Where(
            x => 
                x.DeliveryDocuments.Any(
                    y => y.PartnerCode.IsOneOf(partnerCodes)));
    }
    private static IQueryable<SignedLoadingDocumentView> FilterCombinedRznoCode(IQueryable<SignedLoadingDocumentView> query, string combinedRznoCode)
    {
        return query.Where(
            x => 
                x.DeliveryDocuments.Any(
                    y => 
                        !string.IsNullOrWhiteSpace(y.CombinedRznoCode) 
                        && y.CombinedRznoCode.Contains(combinedRznoCode, StringComparison.OrdinalIgnoreCase)));
    }
    private static IQueryable<SignedLoadingDocumentView> FilterPartnersOrderNumber(IQueryable<SignedLoadingDocumentView> query, string partnersOrderNumber)
    {
        return query.Where(
            x => 
                x.DeliveryDocuments.Any(
                    y => y.PartnersOrderNumber.Contains(partnersOrderNumber, StringComparison.OrdinalIgnoreCase)));
    }
    private static IQueryable<SignedLoadingDocumentView> FilterRznoCode(IQueryable<SignedLoadingDocumentView> query, string rznoCode)
    {
        return query.Where(
            x => 
                x.DeliveryDocuments.Any(
                    y => 
                        !string.IsNullOrWhiteSpace(y.RznoCode) 
                        && y.RznoCode.Contains(rznoCode, StringComparison.OrdinalIgnoreCase)));
    }
    
    private async Task<IQueryable<SignedLoadingDocumentView>> FilterFullTextAsync(IQueryable<SignedLoadingDocumentView> query, string fullTextFilter, CancellationToken cancellationToken)
    {
        var receivers = await sender.Send(new GetReceiverContractsByNameQuery(fullTextFilter), cancellationToken);
        var receiverCodes = receivers.Select(x => x.Code).ToArray();
        
        var partners = await sender.Send(new GetPartnerContractsByNameQuery(fullTextFilter), cancellationToken);
        var partnerCodes = partners.Select(x => x.Code).ToArray();

        var isNumber = decimal.TryParse(fullTextFilter, out var fullTextFilterNumber);
        
        return query.Where(x =>
            // Top-level fields
            (isNumber && x.GateNumber.HasValue && x.GateNumber.Value == fullTextFilterNumber) || 
            (isNumber && x.Weight.HasValue && x.Weight.Value == fullTextFilterNumber) || 
            (isNumber && x.AdrPoints.HasValue && x.AdrPoints.Value == fullTextFilterNumber) || 
            (!string.IsNullOrWhiteSpace(x.Code) && x.Code.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(x.LicensePlate) && x.LicensePlate.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(x.DriverName) && x.DriverName.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(x.DeliveryMethodCode) && x.DeliveryMethodCode.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(x.DeliveryMethodName) && x.DeliveryMethodName.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(x.DepositorCode) && x.DepositorCode.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(x.LoadingLocation) && x.LoadingLocation.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
            // Nested DeliveryDocuments fields (OR)
            x.DeliveryDocuments.Any(y =>
                (!string.IsNullOrWhiteSpace(y.PartnersOrderNumber) && y.PartnersOrderNumber.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(y.RznoCode) && y.RznoCode.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(y.CombinedRznoCode) && y.CombinedRznoCode.Contains(fullTextFilter, StringComparison.OrdinalIgnoreCase)) ||
                receiverCodes.Contains(y.OperationalUnitCode) ||
                partnerCodes.Contains(y.PartnerCode)
            )
        );
    }
}