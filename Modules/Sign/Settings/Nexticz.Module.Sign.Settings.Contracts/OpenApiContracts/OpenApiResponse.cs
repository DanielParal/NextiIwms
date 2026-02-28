namespace Nexticz.Module.Sign.Settings.Contracts.OpenApiContracts;

public record OpenApiResponse(
    ReceivableNotificationContract ReceivableNotification,
    ModuleNameContract ModuleName);