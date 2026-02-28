using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateWashingMachine;

internal class CreateWashingMachineCommandValidator : AbstractValidator<CreateWashingMachineCommand>
{
    public CreateWashingMachineCommandValidator()
    {
        RuleFor(x => x.WashingMachineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationWashingMachineCodeIsRequired));
    }
}