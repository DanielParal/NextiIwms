using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.CallSos;

internal class CallSosCommandValidator : AbstractValidator<CallSosCommand>
{
    public CallSosCommandValidator()
    {
        RuleFor(x => x.WashingMachineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineSosErrors.ValidationCodeIsRequired));
    }
}