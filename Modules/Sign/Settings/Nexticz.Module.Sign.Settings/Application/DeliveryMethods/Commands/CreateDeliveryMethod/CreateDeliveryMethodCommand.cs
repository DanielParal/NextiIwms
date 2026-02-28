using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.CreateDeliveryMethod;

internal record CreateDeliveryMethodCommand(
    string Code, string Name, int LoadingDocumentPrintCopiesCount, int DeliveryDocumentPrintCopiesCount) 
    : ISettingsCommand<ErrorOr<DeliveryMethod>>;