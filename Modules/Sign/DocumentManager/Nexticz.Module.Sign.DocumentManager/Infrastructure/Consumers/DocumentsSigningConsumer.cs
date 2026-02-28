using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

internal class DocumentsSigningConsumer(
    ISignDocumentsOrchestrator signDocumentsOrchestrator,
    ILogger<DocumentsSigningConsumer> logger,
    IDocumentSigningNotifier signingNotifier
    ) : IConsumer<SignDocumentsRequested>
{
    public async Task Consume(ConsumeContext<SignDocumentsRequested> context)
    {
        try
        {
            logger.LogInformation("[Start] [DocumentsSigningConsumer]");
            var content = await context.Message.SignatureImage.Content.Value;
            var fileResult = new FileResult(content, context.Message.SignatureImage.ContentType, context.Message.SignatureImage.FileName);
            await signDocumentsOrchestrator.SignAsync(
                context.Message.SigningDeviceCode,
                context.Message.CurrentUserName,
                context.Message.DriverName,
                context.Message.LicensePlate,
                fileResult,
                CancellationToken.None);
            
            logger.LogInformation("[End] [DocumentsSigningConsumer]");
        }
        catch (Exception ex)
        {
            await signingNotifier.NotifyDocumentsSigningFailedUnexpectedlyAsync(context.Message.SigningDeviceCode, [], CancellationToken.None);
            logger.LogError(ex, "[Error] [DocumentsSigningConsumer]");
        }
        
    }
}