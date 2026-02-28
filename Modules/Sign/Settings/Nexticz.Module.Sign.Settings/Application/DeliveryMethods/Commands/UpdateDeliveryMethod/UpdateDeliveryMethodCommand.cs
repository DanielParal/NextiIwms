using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;

internal record UpdateDeliveryMethodCommand(
    string Code, string Name, int LoadingDocumentPrintCopiesCount, int DeliveryDocumentPrintCopiesCount) 
    : ISettingsCommand<ErrorOr<Updated>>;