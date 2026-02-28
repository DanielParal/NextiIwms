using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.CreateDepositorGroup;

internal class CreateDepositorGroupCommandValidator : AbstractValidator<CreateDepositorGroupCommand>
{
    public CreateDepositorGroupCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorGroupErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorGroupErrors.ValidationNameIsRequired));
    }
}