using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates.Queries;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;

internal class DeliveryDocumentEmailRenderer(ISender sender, IClock clock) : IDeliveryDocumentEmailRenderer
{
    private readonly List<(string Code, EmailTemplateContract EmailTemplate)> _emailTemplateCache = [];
    
    public async Task<ErrorOr<RenderedEmailResult>> RenderEmailAsync(DeliveryDocument deliveryDocument, string? depositorName, DateTimeOffset loadingFinishedInWmsAt, CancellationToken cancellationToken)
    {
        var emailTemplateContract = await GetEmailTemplateContractAsync(cancellationToken);
        
        if (emailTemplateContract.IsError)
            return emailTemplateContract.Errors;
        
        var renderedSubject = GetSubject(emailTemplateContract.Value.Subject, deliveryDocument);
        var renderedHtmlBody = ReplaceMainData(emailTemplateContract.Value.HtmlBody, deliveryDocument, depositorName, loadingFinishedInWmsAt);
        var renderedTxtBody = ReplaceMainData(emailTemplateContract.Value.TextBody, deliveryDocument, depositorName, loadingFinishedInWmsAt);
        
        return new  RenderedEmailResult(renderedSubject, renderedHtmlBody, renderedTxtBody);
    }
    
    private static string GetSubject(string rawSubject, DeliveryDocument deliveryDocument)
    {
        return rawSubject.Replace("{{ORDER_NUMBER}}", deliveryDocument.PartnersOrderNumber);
    }
    
    private string ReplaceMainData(string rawData, DeliveryDocument deliveryDocument, string? depositorName, DateTimeOffset loadingFinishedInWmsAt)
    {
        return rawData
            .Replace("{{DEPOSITOR_CODE}}", depositorName)
            .Replace("{{ORDER_NUMBER}}", deliveryDocument.PartnersOrderNumber)
            .Replace("{{DELIVERY_DOCUMENT_CODE}}", deliveryDocument.Code)
            .Replace("{{RZNO_CODE}}", deliveryDocument.RznoCode)
            .Replace("{{RZNO_CODE_COMBINED}}", deliveryDocument.CombinedRznoCode)
            .Replace("{{PARTNER}}", deliveryDocument.PartnerNameShort)
            .Replace("{{RECEIVER}}", deliveryDocument.OperationalUnitName)
            .Replace("{{LOADING_DOCUMENT_CODE}}", deliveryDocument.LoadingDocumentCode)
            .Replace("{{LOADING_DATE}}", clock.ConvertUtcToTenantDateTime(loadingFinishedInWmsAt).ToString("dd.MM.yyyy"))
            .Replace("{{DRIVER_NAME}}", deliveryDocument.SignedByDriverName)
            .Replace("{{LICENSE_PLATE}}", deliveryDocument.SignedWithLicensePlate)
            .Replace("{{WEIGHT}}", deliveryDocument.WeightCalculated?.ToString())
            .Replace("{{ADR_POINTS}}", deliveryDocument.AdrPoints?.ToString());
    }
    
    private async Task<ErrorOr<EmailTemplateContract>> GetEmailTemplateContractAsync(CancellationToken cancellationToken)
    {
        var upperEmailTemplateCode = nameof(EmailTemplateTypeContract.DeliveryDocumentTemplate).ToUpperInvariant();
        var cachedTemplate = _emailTemplateCache
            .FirstOrDefault(x => 
                x.Code.Equals(upperEmailTemplateCode, StringComparison.InvariantCultureIgnoreCase));

        if (cachedTemplate != default)
            return cachedTemplate.EmailTemplate;
        
        var emailTemplateContract = await sender.Send(new GetEmailTemplateContractByCodeQuery(upperEmailTemplateCode), cancellationToken);

        if (emailTemplateContract.IsError)
            return emailTemplateContract.Errors;
        
        _emailTemplateCache.Add((upperEmailTemplateCode, emailTemplateContract.Value));

        return emailTemplateContract.Value;
    }
}