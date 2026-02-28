using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record SignDocumentsResponse([property: Required] DateTimeOffset PrintDeadline);