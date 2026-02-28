using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.CreateLoadingDocument;

internal class CreateLoadingDocumentCommandHandler(
        ILogger<CreateLoadingDocumentCommandHandler> logger,
        IDocumentManagerUnitOfWork unitOfWork,
        IClock clock,
        ISender sender
    ) : IRequestHandler<CreateLoadingDocumentCommand, ErrorOr<LoadingDocument>>
{
    public async Task<ErrorOr<LoadingDocument>> Handle(CreateLoadingDocumentCommand request, CancellationToken cancellationToken)
    {
        var deliveryMethodCodes = new[] { request.LoadingDocumentContractFromLoader.DeliveryMethodCode }
            .Union(request.LoadingDocumentContractFromLoader.DeliveryNotes.Select(dn => dn.DeliveryMethodCode))
            .Distinct()
            .ToArray();
        
        var deliveryMethodContracts = await sender.Send(new GetDeliveryMethodContractsByCodesQuery(deliveryMethodCodes), cancellationToken);
        var requestedPrintCopiesCountForLoadingDocument = GetRequestedPrintCopiesCount(true, request.LoadingDocumentContractFromLoader.DeliveryMethodCode, deliveryMethodContracts);
        var depositorName = await GetDepositorNameAsync(request.LoadingDocumentContractFromLoader.DepositorCode, cancellationToken);
        
        var loadingDocument = new LoadingDocument(
            request.LoadingDocumentContractFromLoader.Code,
            request.LoadingDocumentContractFromLoader.GateNumber,
            request.LoadingDocumentContractFromLoader.DepositorCode,
            depositorName,
            request.LoadingDocumentContractFromLoader.DeliveryMethodCode,
            GetDeliveryMethodContract(request.LoadingDocumentContractFromLoader.DeliveryMethodCode, deliveryMethodContracts)?.Name,
            request.LoadingDocumentContractFromLoader.LicensePlate,
            request.LoadingDocumentContractFromLoader.DriverName,
            RoundWeight(request.LoadingDocumentContractFromLoader.Weight),
            request.LoadingDocumentContractFromLoader.AdrPoints,
            request.LoadingDocumentContractFromLoader.LoadingLocation,
            request.LoadingDocumentContractFromLoader.LoadingInWmsFinishedBy,
            clock.ConvertTenantToUtcDateTime(request.LoadingDocumentContractFromLoader.LoadingInWmsFinishedAt),
            clock.TenantNowOffset,
            requestedPrintCopiesCountForLoadingDocument,
            request.LoadingDocumentContractFromLoader.DeliveryNotes
                .Select(dn => new DeliveryDocument(
                    dn.Code,
                    dn.LoadingDocumentCode,
                    dn.PartnerCode,
                    dn.PartnerNameShort,
                    dn.DeliveryMethodCode,
                    GetDeliveryMethodContract(dn.DeliveryMethodCode, deliveryMethodContracts)?.Name,
                    dn.WarehouseCode,
                    dn.PartnersOrderNumber,
                    dn.OperationalUnitCode,
                    dn.OperationalUnitName,
                    DateOnly.FromDateTime(dn.IssueDate),
                    RoundWeight(dn.WeightCalculated),
                    dn.AdrPoints,
                    dn.RznoCode,
                    dn.CombinedRznoCode,
                    GetRequestedPrintCopiesCount(false, dn.DeliveryMethodCode, deliveryMethodContracts)
                ))
                .ToArray());
        
        var loadingDocumentCreatedEvent =
            new LoadingDocumentCreatedEvent(
                loadingDocument.Id, loadingDocument.Code, loadingDocument.GateNumber, loadingDocument.DepositorCode, loadingDocument.DepositorName,
                loadingDocument.DeliveryMethodCode, loadingDocument.DeliveryMethodName, loadingDocument.OriginalLicensePlate, loadingDocument.OriginalDriverName, 
                loadingDocument.Weight, loadingDocument.AdrPoints, loadingDocument.LoadingLocation, loadingDocument.LoadingInWmsFinishedBy,
                loadingDocument.LoadingInWmsFinishedAt, loadingDocument.CreatedAt, requestedPrintCopiesCountForLoadingDocument, loadingDocument.DeliveryDocuments);

        unitOfWork.StartStream<LoadingDocumentCreatedEvent, LoadingDocument>(
            loadingDocument.Id, loadingDocumentCreatedEvent);
            
        logger.LogInformation("SIGN - DocumentManager - loading document with code: {LoadingListCode} created with number of delivery notes: {DeliveryNotesCount}.", 
            loadingDocument.Code, loadingDocument.DeliveryDocuments.Length);
        
        return loadingDocument;
    }
    
    private static int GetRequestedPrintCopiesCount(bool isLoadingDocument, string documentDeliveryMethodCode, DeliveryMethodContract[] deliveryMethodContracts)
    {
        var deliveryMethod = GetDeliveryMethodContract(documentDeliveryMethodCode, deliveryMethodContracts);
        
        const int defaultPrintCopiesCount = 1;
        if (deliveryMethod is null)
            return defaultPrintCopiesCount;
        
        return isLoadingDocument ? deliveryMethod.LoadingDocumentPrintCopiesCount : deliveryMethod.DeliveryDocumentPrintCopiesCount;
    }

    private static DeliveryMethodContract? GetDeliveryMethodContract(string deliveryMethodCode,
        DeliveryMethodContract[] deliveryMethodContracts)
            => deliveryMethodContracts.FirstOrDefault(x => x.Code == deliveryMethodCode);
    
    private async Task<string?> GetDepositorNameAsync(string depositorCode, CancellationToken cancellationToken)
    {
        var depositorResponse = await sender.Send(new GetDepositorResponseByCodeQuery(depositorCode), cancellationToken);
        return depositorResponse.IsError ? null : depositorResponse.Value.Name;
    }

    private static decimal? RoundWeight(decimal? weightCalculated)
        => weightCalculated.HasValue ? Math.Round(weightCalculated.Value, 2) : null;
}