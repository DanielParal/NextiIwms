using Microsoft.AspNetCore.Http;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SigningPublishers;

internal interface IDocumentManagerSigningPublisher
{
    Task PublishDocumentsSigningAsync(string signingDeviceCode, string currentUserName, string driverName, string licensePlate,
        IFormFile signatureFile, CancellationToken cancellationToken);
}