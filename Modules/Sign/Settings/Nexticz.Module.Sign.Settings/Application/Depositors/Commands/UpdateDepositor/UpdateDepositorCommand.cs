using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.UpdateDepositor;

internal record UpdateDepositorCommand(
    string Code, string Name, string DepositorGroupCode, string DeliveryTemplateCode, string LoadingTemplateCode) 
    : ISettingsCommand<ErrorOr<Updated>>;