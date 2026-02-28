using MassTransit;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record SignDocumentsRequested(
    string SigningDeviceCode, 
    string CurrentUserName,
    string DriverName, 
    string LicensePlate,
    Signature SignatureImage
    );
    
public record Signature(
    MessageData<byte[]> Content,
    string FileName,
    string ContentType,
    long Length,
    string Sha256
    );