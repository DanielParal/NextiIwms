using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.UpdateWashingMachineStatus;

internal class UpdateWashingMachineStatusCommandValidator : AbstractValidator<UpdateWashingMachineStatusCommand>
{
    public UpdateWashingMachineStatusCommandValidator()
    {
        RuleFor(x => x.WashingMachineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationWashingMachineCodeIsRequired));
    }
}