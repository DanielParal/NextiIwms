using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.CreateDepositor;

internal record CreateDepositorCommand(
    string Code, string Name, string DepositorGroupCode, string DeliveryTemplateCode, string LoadingTemplateCode) 
    : ISettingsCommand<ErrorOr<Depositor>>;