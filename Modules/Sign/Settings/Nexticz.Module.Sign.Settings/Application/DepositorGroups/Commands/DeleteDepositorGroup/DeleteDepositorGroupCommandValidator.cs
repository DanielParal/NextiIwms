using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.DeleteDepositorGroup;

internal class DeleteDepositorGroupCommandValidator : AbstractValidator<DeleteDepositorGroupCommand>
{
    public DeleteDepositorGroupCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorGroupErrors.ValidationCodeIsRequired));
    }
}