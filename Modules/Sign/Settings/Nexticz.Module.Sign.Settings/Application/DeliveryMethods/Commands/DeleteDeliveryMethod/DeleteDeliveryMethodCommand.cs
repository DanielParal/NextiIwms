
using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;

internal record DeleteDeliveryMethodCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;