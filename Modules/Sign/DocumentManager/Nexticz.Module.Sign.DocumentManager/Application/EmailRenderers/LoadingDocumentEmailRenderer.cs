using System.Text;
using System.Text.RegularExpressions;
using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates.Queries;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;

internal class LoadingDocumentEmailRenderer(ISender sender) : ILoadingDocumentEmailRenderer
{
    private readonly List<(string Code, EmailTemplateContract EmailTemplate)> _emailTemplateCache = [];
    
    public async Task<ErrorOr<RenderedEmailResult>> RenderEmailAsync(LoadingDocument loadingDocument, CancellationToken cancellationToken)
    {
        var emailTemplateContract = await GetEmailTemplateContractAsync(cancellationToken);
        
        if (emailTemplateContract.IsError)
            return emailTemplateContract.Errors;
        
        var renderedSubject = GetSubject(emailTemplateContract.Value.Subject, loadingDocument);
        var renderedHtmlBody = GetHtmlBody(emailTemplateContract.Value.HtmlBody, loadingDocument);
        var renderedTxtBody = GetTextBody(emailTemplateContract.Value.TextBody, loadingDocument);
        
        return new  RenderedEmailResult(renderedSubject, renderedHtmlBody, renderedTxtBody);
    }

    private static string GetSubject(string rawSubject, LoadingDocument loadingDocument)
    {
        return rawSubject.Replace("{{LOADING_DOCUMENT_CODE}}", loadingDocument.Code);
    }

    private static string GetHtmlBody(string rawHtmlBody, LoadingDocument loadingDocument)
    {
        var renderedHtmlBody = ReplaceMainData(rawHtmlBody, loadingDocument);
        
        return ReplaceHtmlDeliveryDocumentsData(renderedHtmlBody, loadingDocument.DeliveryDocuments);
    }

    private static string ReplaceHtmlDeliveryDocumentsData(string  htmlBody, DeliveryDocument[] deliveryDocuments)
    {
        const string pattern = @"\{\{#DELIVERY_DOCUMENTS\}\}([\s\S]*?)\{\{\/DELIVERY_DOCUMENTS\}\}";
        var match = Regex.Match(htmlBody, pattern);

        if (!match.Success)
            return htmlBody;

        var rowTemplate = match.Groups[1].Value.Trim();
        
        var rowsBuilder = new StringBuilder();

        foreach (var deliveryDocument in deliveryDocuments)
        {
            var currentRow = rowTemplate
                .Replace("{{LOOP_ORDER_NUMBER}}", deliveryDocument.PartnersOrderNumber)
                .Replace("{{LOOP_DELIVERY_DOCUMENT_CODE}}", deliveryDocument.Code)
                .Replace("{{LOOP_RZNO_CODE}}", deliveryDocument.RznoCode)
                .Replace("{{LOOP_RZNO_CODE_COMBINED}}", deliveryDocument.CombinedRznoCode)
                .Replace("{{LOOP_PARTNER}}", deliveryDocument.PartnerNameShort)
                .Replace("{{LOOP_RECEIVER}}", deliveryDocument.OperationalUnitName)
                .Replace("{{LOOP_WEIGHT}}", deliveryDocument.WeightCalculated.ToString())
                .Replace("{{LOOP_ADR_POINTS}}", deliveryDocument.AdrPoints.ToString());

            rowsBuilder.AppendLine(currentRow);
        }
        
        return Regex.Replace(htmlBody, pattern, rowsBuilder.ToString());
    }

    private static string GetTextBody(string rawTextBody, LoadingDocument loadingDocument)
    {
        var renderedTextBody = ReplaceMainData(rawTextBody, loadingDocument);
        return ReplaceTxtDeliveryDocumentsData(renderedTextBody, loadingDocument.DeliveryDocuments);
    }

    private static string ReplaceTxtDeliveryDocumentsData(string textBody, DeliveryDocument[] deliveryDocuments)
    {
        const string pattern = @"\{\{DELIVERY_DOCUMENTS_LOOP\}\}";
        var match = Regex.Match(textBody, pattern);

        if (!match.Success)
            return textBody;
        
        var rowsBuilder = new StringBuilder();

        foreach (var deliveryDocument in deliveryDocuments)
        {
            var currentRow = 
                $"{deliveryDocument.PartnersOrderNumber} | " +
                $"{deliveryDocument.Code} | " +
                $"{deliveryDocument.RznoCode} | " +
                $"{deliveryDocument.CombinedRznoCode} | " +
                $"{deliveryDocument.PartnerCode} | " +
                $"{deliveryDocument.OperationalUnitCode} | " +
                $"{deliveryDocument.WeightCalculated} | " +
                $"{deliveryDocument.AdrPoints}";

            rowsBuilder.AppendLine(currentRow);
        }
        
        return Regex.Replace(textBody, pattern, rowsBuilder.ToString());
    }

    private static string ReplaceMainData(string rawData, LoadingDocument loadingDocument)
    {
        return rawData
                .Replace("{{DEPOSITOR_CODE}}", loadingDocument.DepositorName)
                .Replace("{{LOADING_DOCUMENT_CODE}}", loadingDocument.Code)
                .Replace("{{LOADING_DATE}}", loadingDocument.LoadingInWmsFinishedAt.ToString("dd.MM.yyyy"))
                .Replace("{{DRIVER_NAME}}", loadingDocument.SignedByDriverName)
                .Replace("{{LICENSE_PLATE}}", loadingDocument.SignedWithLicensePlate)
                .Replace("{{TOTAL_WEIGHT}}", loadingDocument.Weight?.ToString())
                .Replace("{{ADR_POINTS}}", loadingDocument.AdrPoints?.ToString());
    }

    private async Task<ErrorOr<EmailTemplateContract>> GetEmailTemplateContractAsync(CancellationToken cancellationToken)
    {
        var upperEmailTemplateCode = nameof(EmailTemplateTypeContract.LoadingDocumentTemplate).ToUpperInvariant();
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