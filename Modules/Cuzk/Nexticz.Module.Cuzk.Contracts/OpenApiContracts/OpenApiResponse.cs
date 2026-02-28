using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Cuzk.Contracts.OpenApiContracts;

public record OpenApiResponse(
    [property: Required] ReceivableNotificationContract ReceivableNotification);