using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.UpdateDepositor;

internal class UpdateDepositorCommandValidator : AbstractValidator<UpdateDepositorCommand>
{
    public UpdateDepositorCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorErrors.ValidationNameIsRequired));
    }
}