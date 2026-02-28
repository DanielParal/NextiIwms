using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.UpdateDepositorGroup;

internal class UpdateDepositorGroupCommandValidator : AbstractValidator<UpdateDepositorGroupCommand>
{
    public UpdateDepositorGroupCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorGroupErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorGroupErrors.ValidationNameIsRequired));
    }
}