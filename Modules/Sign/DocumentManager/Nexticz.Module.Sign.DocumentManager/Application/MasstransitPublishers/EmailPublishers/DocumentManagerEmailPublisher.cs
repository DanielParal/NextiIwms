using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations.Queries;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;
using Nexticz.Module.EmailSender.Publishers;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.DocumentManager.Application.EmailRenderers;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.EmailPublishers;

internal class DocumentManagerEmailPublisher(
    IDocumentManagerPublisher messagePublisher, 
    IDocumentManagerFileHandler fileHandler, 
    AssetsSettings assetsSettings,
    ILogger<DocumentManagerEmailPublisher> logger,
    ILoadingDocumentEmailRenderer loadingDocumentEmailRenderer,
    IDeliveryDocumentEmailRenderer deliveryDocumentEmailRenderer,
    ISender sender,
    IClock clock) : EmailPublisher(messagePublisher, fileHandler, assetsSettings, logger), IDocumentManagerEmailPublisher
{
    private readonly TimeOnly _scheduledBulkSendTime = new(22, 0);
    private readonly List<(string DepositorCode, string PartnerCode, string ReceiverCode, EmailConfigurationContract[] Configurations)> _deliveryEmailConfigurationCache = [];
    private readonly List<(string DepositorCode, EmailConfigurationContract Configuration)> _loadingEmailConfigurationCache = [];
    private readonly AssetsSettings _assetsSettings = assetsSettings;

    public async Task PublishEmailsBasedOnConfigurationAsync(DocumentEmailMessage[] documentEmailMessages, CancellationToken cancellationToken)
    {
        if (documentEmailMessages.Length == 0)
            return;

        foreach (var documentEmailMessage in documentEmailMessages)
        {
            if (documentEmailMessage.ShouldLoadingDocumentBeSent)
            {
                var loadingDocumentWithFilteredDeliveryDocuments = 
                    documentEmailMessage
                        .LoadingDocument
                        .WithFilteredAndSignedDeliveryDocuments(documentEmailMessage.DeliveryDocumentCodesToSend);
                
                await PublishLoadingEmailsBasedOnConfigurationsAsync(
                    loadingDocumentWithFilteredDeliveryDocuments,
                    cancellationToken);
            }

            foreach (var deliveryDocumentCode in documentEmailMessage.DeliveryDocumentCodesToSend)
            {
                var deliveryDocument = 
                    documentEmailMessage
                        .LoadingDocument
                        .DeliveryDocuments
                        .FirstOrDefault(x => x.Code == deliveryDocumentCode);
                
                if (deliveryDocument == null)
                    continue;
                
                await PublishDeliveryEmailsBasedOnConfigurationsAsync(
                    documentEmailMessage.LoadingDocument,
                    deliveryDocument,
                    cancellationToken);
            }
        }
    }

    public async Task PublishEmailsAsync(string[] recipients, DocumentEmailMessage[] documentEmailMessages,
        CancellationToken cancellationToken)
    {
        if (recipients.Length == 0)
            return;
        
        if (documentEmailMessages.Length == 0)
            return;
        
        foreach (var documentEmailMessage in documentEmailMessages)
        {
            // you either send NL with filtered delivery documents in one email or no NL but all delivery documents in separate emails  
            if (documentEmailMessage.ShouldLoadingDocumentBeSent)
            {
                var loadingDocumentWithFilteredDeliveryDocuments = 
                    documentEmailMessage
                        .LoadingDocument
                        .WithFilteredDeliveryDocuments(documentEmailMessage.DeliveryDocumentCodesToSend);
                
                await PublishLoadingEmailsAsync(
                    recipients,
                    loadingDocumentWithFilteredDeliveryDocuments,
                    cancellationToken);
                
                continue;        
            }

            foreach (var deliveryDocumentCode in documentEmailMessage.DeliveryDocumentCodesToSend)
            {
                var deliveryDocument = 
                    documentEmailMessage
                        .LoadingDocument
                        .DeliveryDocuments
                        .FirstOrDefault(x => x.Code == deliveryDocumentCode);
                
                if (deliveryDocument == null)
                    continue;
                
                await PublishDeliveryEmailsAsync(
                    recipients,
                    documentEmailMessage.LoadingDocument,
                    deliveryDocument,
                    cancellationToken);
            }
        }
    }

    public async Task PublishEmailsAboutMissingDepositorAsync(string[] recipients, string depositorCode, string loadingDocumentCode,
        string createdByUserNameInWms, CancellationToken cancellationToken)
    {
        var text = $"iWMS - SIGN Aplikace - chybějící ukladatel: {depositorCode} pro dokument: {loadingDocumentCode}, který vytvořil: {createdByUserNameInWms}";
        var renderedEmail = new RenderedEmailResult(text, text, text);
        
        foreach (var recipient in recipients)
        {
            var publishEmailMessage =
                new PublishEmailMessage(
                    [recipient],
                    [],
                    [],
                    renderedEmail.Subject,
                    renderedEmail.HtmlBody,
                    renderedEmail.TextBody,
                    ModuleNameProvider.Name,
                    "SignEmailsForMissingDepositor",
                    false,
                    new Dictionary<string, string>(),
                    [],
                    null
                );
            
            await PublishSendEmailAsync(publishEmailMessage, cancellationToken);
        }
    }

    public async Task PublishEmailsAboutMissingDeliveryMethodAsync(string[] recipients, string[] deliveryMethodCodes,
        string loadingDocumentCode, string createdByUserNameInWms, CancellationToken cancellationToken)
    {
        var text = $"iWMS - SIGN Aplikace - chybějící způsoby dodání: [{string.Join(',', deliveryMethodCodes)}] pro dokument: {loadingDocumentCode}, který vytvořil: {createdByUserNameInWms}";
        var renderedEmail = new RenderedEmailResult(text, text, text);
        
        foreach (var recipient in recipients)
        {
            var publishEmailMessage =
                new PublishEmailMessage(
                    [recipient],
                    [],
                    [],
                    renderedEmail.Subject,
                    renderedEmail.HtmlBody,
                    renderedEmail.TextBody,
                    ModuleNameProvider.Name,
                    "SignEmailsForMissingDeliveryMethod",
                    false,
                    new Dictionary<string, string>(),
                    [],
                    null
                );
            
            await PublishSendEmailAsync(publishEmailMessage, cancellationToken);
        }
    }

    private async Task PublishLoadingEmailsBasedOnConfigurationsAsync(
        LoadingDocument loadingDocument,
        CancellationToken cancellationToken)
    {
        var emailConfiguration = await GetLoadingEmailConfigurationContractAsync(loadingDocument.DepositorCode, cancellationToken);

        if (emailConfiguration == null)
            return;
        
        var initiatorProperties = GetEmailInitiatorProperties(loadingDocument.Code, null);
        
        var renderedEmail = await loadingDocumentEmailRenderer.RenderEmailAsync(loadingDocument, cancellationToken);
            
        if (renderedEmail.IsError)
        {
            logger.LogError("Sign - Document manager - cannot render email tamplate for loading document: {LoadingDocument}.",
                loadingDocument.Code);
            return;
        }
            
        var attachments = GetLoadingDocumentAttachments(loadingDocument, emailConfiguration.ShouldSendLoadingDocument, emailConfiguration.ShouldSendDeliveryDocument);
            
        await PublishEmailMessagesAsync(
            nameof(EmailTemplateTypeContract.LoadingDocumentTemplate),
            emailConfiguration.ShouldSendImmediately,
            emailConfiguration.RecipientEmailAddresses,
            renderedEmail.Value,
            initiatorProperties,
            attachments,
            cancellationToken);
    }
    
    private async Task PublishLoadingEmailsAsync(
        string[] recipients,
        LoadingDocument loadingDocument,
        CancellationToken cancellationToken)
    {
        var initiatorProperties = GetEmailInitiatorProperties(loadingDocument.Code, null);
        
        var renderedEmail = await loadingDocumentEmailRenderer.RenderEmailAsync(loadingDocument, cancellationToken);
            
        if (renderedEmail.IsError)
        {
            logger.LogError("Sign - Document manager - cannot render email tamplate for loading document: {LoadingDocument}.",
                loadingDocument.Code);
            return;
        }
            
        var attachments = GetLoadingDocumentAttachments(loadingDocument, true, true);
            
        await PublishEmailMessagesAsync(
            nameof(EmailTemplateTypeContract.LoadingDocumentTemplate),
            true,
            recipients,
            renderedEmail.Value,
            initiatorProperties,
            attachments,
            cancellationToken);
    }

    private async Task PublishDeliveryEmailsBasedOnConfigurationsAsync(
        LoadingDocument loadingDocument, 
        DeliveryDocument deliveryDocument,
        CancellationToken cancellationToken)
    {
        var emailConfigurations =
            await GetDeliveryEmailConfigurationContractsAsync(
                loadingDocument.DepositorCode,
                deliveryDocument.PartnerCode,
                deliveryDocument.OperationalUnitCode,
                cancellationToken);
        
        var initiatorProperties = GetEmailInitiatorProperties(loadingDocument.Code, deliveryDocument.Code);
        
        foreach (var emailConfiguration in emailConfigurations)
        {
            var renderedEmail = await deliveryDocumentEmailRenderer.RenderEmailAsync(
                    deliveryDocument, loadingDocument.DepositorName, loadingDocument.LoadingInWmsFinishedAt, cancellationToken);
            
            if (renderedEmail.IsError)
            {
                logger.LogError("Sign - Document manager - cannot render email tamplete for loading document: {LoadingDocument} and delivery document: {DeliveryDocument}.",
                    loadingDocument.Code, deliveryDocument.Code);
                continue;
            }
            
            PublishEmailAttachment[] attachments = emailConfiguration.ShouldSendDeliveryDocument ? [GetDeliveryDocumentAttachment(loadingDocument.Code, deliveryDocument.Code, loadingDocument.DepositorCode)] : [];

            await PublishEmailMessagesAsync(
                nameof(EmailTemplateTypeContract.DeliveryDocumentTemplate),
                emailConfiguration.ShouldSendImmediately,
                emailConfiguration.RecipientEmailAddresses,
                renderedEmail.Value,
                initiatorProperties,
                attachments,
                cancellationToken);
        }
    }
    
    private async Task PublishDeliveryEmailsAsync(
        string[] recipients,
        LoadingDocument loadingDocument, 
        DeliveryDocument deliveryDocument,
        CancellationToken cancellationToken)
    {
        var initiatorProperties = GetEmailInitiatorProperties(loadingDocument.Code, deliveryDocument.Code);
        
        var renderedEmail = await deliveryDocumentEmailRenderer.RenderEmailAsync(
            deliveryDocument, loadingDocument.DepositorName, loadingDocument.LoadingInWmsFinishedAt, cancellationToken);
            
        if (renderedEmail.IsError)
        {
            logger.LogError("Sign - Document manager - cannot render email tamplete for loading document: {LoadingDocument} and delivery document: {DeliveryDocument}.",
                loadingDocument.Code, deliveryDocument.Code);
            return;
        }
            
        PublishEmailAttachment[] attachments = [GetDeliveryDocumentAttachment(loadingDocument.Code, deliveryDocument.Code, loadingDocument.DepositorCode)];

        await PublishEmailMessagesAsync(
            nameof(EmailTemplateTypeContract.DeliveryDocumentTemplate),
            true,
            recipients,
            renderedEmail.Value,
            initiatorProperties,
            attachments,
            cancellationToken);
    }

    private async Task PublishEmailMessagesAsync(string emailType, bool shouldSendEmailImmediately, string[] recipients, RenderedEmailResult renderedEmail, 
        Dictionary<string, string> initiatorProperties, PublishEmailAttachment[] attachments, CancellationToken cancellationToken)
    {
        DateTimeOffset? scheduledAt = shouldSendEmailImmediately ? null : clock.GetTodayWithTenantTime(_scheduledBulkSendTime);
        foreach (var recipient in recipients)
        {
            var publishEmailMessage =
                new PublishEmailMessage(
                    [recipient],
                    [],
                    [],
                    renderedEmail.Subject,
                    renderedEmail.HtmlBody,
                    renderedEmail.TextBody,
                    ModuleNameProvider.Name,
                    emailType,
                    true,
                    initiatorProperties,
                    attachments,
                    scheduledAt
                );
            
            await PublishSendEmailAsync(publishEmailMessage, cancellationToken);
        }
    }

    private PublishEmailAttachment[] GetLoadingDocumentAttachments(LoadingDocument loadingDocument, bool shouldSendLoadingDocument, bool shouldSendDeliveryDocuments)
    {
        var attachments = new List<PublishEmailAttachment>();

        if (shouldSendLoadingDocument)
        {
            attachments.Add(GetLoadingDocumentAttachment(loadingDocument.Code, loadingDocument.DepositorCode));
        }
        
        if (!shouldSendDeliveryDocuments)
            return attachments.ToArray();


        foreach (var deliveryDocument in loadingDocument.DeliveryDocuments)
        {
            attachments.Add(GetDeliveryDocumentAttachment(loadingDocument.Code, deliveryDocument.Code, loadingDocument.DepositorCode));
        }
        
        return attachments.ToArray();
    }
    
    private PublishEmailAttachment GetDeliveryDocumentAttachment(string loadingDocumentCode, string deliveryDocumentCode, string depositorCode)
    {
        return new PublishEmailAttachment(
            $"{deliveryDocumentCode}.pdf",
            $"{clock.TenantNowOffset:yyMMdd}_{depositorCode}_{deliveryDocumentCode}.pdf",
            DirectoryNamesProvider.GetHistoryDeliveryDocumentFilePath(_assetsSettings, loadingDocumentCode, deliveryDocumentCode),
            "application/pdf");
    }
    
    private PublishEmailAttachment GetLoadingDocumentAttachment(string loadingDocumentCode, string depositorCode)
    {
        return new PublishEmailAttachment(
            $"{loadingDocumentCode}.pdf",
            $"{clock.TenantNowOffset:yyMMdd}_{depositorCode}_{loadingDocumentCode}.pdf",
            DirectoryNamesProvider.GetHistoryLoadingDocumentFilePath(_assetsSettings, loadingDocumentCode),
            "application/pdf");
    }
    
    private async Task<EmailConfigurationContract[]> GetDeliveryEmailConfigurationContractsAsync(string depositorCode, string partnerCode, string receiverCode, CancellationToken cancellationToken)
    {
        var cachedConfiguration = _deliveryEmailConfigurationCache
            .FirstOrDefault(x => 
                x.DepositorCode == depositorCode && 
                x.PartnerCode == partnerCode && 
                x.ReceiverCode == receiverCode);

        if (cachedConfiguration != default)
            return cachedConfiguration.Configurations;

        var configurationContracts = await sender.Send(new GetDeliveryEmailConfigurationContractsByCodesQuery(depositorCode, partnerCode, receiverCode), cancellationToken);

        _deliveryEmailConfigurationCache.Add((depositorCode, partnerCode, receiverCode, configurationContracts));

        return configurationContracts;
    }
    
    private async Task<EmailConfigurationContract?> GetLoadingEmailConfigurationContractAsync(string depositorCode, CancellationToken cancellationToken)
    {
        var cachedConfiguration = _loadingEmailConfigurationCache
            .FirstOrDefault(x => 
                x.DepositorCode == depositorCode);

        if (cachedConfiguration != default)
            return cachedConfiguration.Configuration;

        var configurationContract = await sender.Send(new GetLoadingEmailConfigurationContractByDepositorCodeQuery(depositorCode), cancellationToken);

        if (configurationContract.IsError)
            return null;
        
        _loadingEmailConfigurationCache.Add((depositorCode, configurationContract.Value));

        return configurationContract.Value;
    }

    private static Dictionary<string, string> GetEmailInitiatorProperties(string loadingDocumentCode, string? deliveryDocumentCode)
    {
        return new Dictionary<string, string>
        {
            { nameof(EmailInitiatorProperties.LoadingDocumentCode), loadingDocumentCode },
            { nameof(EmailInitiatorProperties.DeliveryDocumentCode), deliveryDocumentCode ?? string.Empty }
        };
    }
}