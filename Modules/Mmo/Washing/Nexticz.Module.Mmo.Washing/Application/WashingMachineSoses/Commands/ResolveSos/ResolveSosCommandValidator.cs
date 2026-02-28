using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.ResolveSos;

internal class ResolveSosCommandValidator : AbstractValidator<ResolveSosCommand>
{
    public ResolveSosCommandValidator()
    {
        RuleFor(x => x.WashingMachineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineSosErrors.ValidationCodeIsRequired));
    }
}