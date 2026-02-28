using MassTransit;
using Microsoft.AspNetCore.Http;
using Nexticz.Module.Sign.DocumentManager.Application.FileHandling;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SigningPublishers;

internal class DocumentManagerSigningPublisher(
    IDocumentManagerPublisher messagePublisher,
    IDocumentManagerFileHandler fileHandler,
    IMessageDataRepository messageDataRepository) : IDocumentManagerSigningPublisher
{
    public async Task PublishDocumentsSigningAsync(string signingDeviceCode, string currentUserName, string driverName, string licensePlate,
        IFormFile signatureFile, CancellationToken cancellationToken)
    {
        var fileBytes = await fileHandler.ReadAllBytesAsync(signatureFile, cancellationToken);
        var contentFile = await messageDataRepository.PutBytes(fileBytes, cancellationToken);
        var sha256 = fileHandler.ComputeSha256Hash(fileBytes);
        
        var message = new SignDocumentsRequested(
            signingDeviceCode,
            currentUserName,
            driverName,
            licensePlate,
            new Signature(
                contentFile, 
                signatureFile.FileName, 
                signatureFile.ContentType, 
                signatureFile.Length, 
                sha256));
        
        await messagePublisher.PublishAsync(message, cancellationToken);
    }
}