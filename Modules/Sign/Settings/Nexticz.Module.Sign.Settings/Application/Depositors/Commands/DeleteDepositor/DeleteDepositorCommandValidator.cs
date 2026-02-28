using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands.DeleteDepositor;

internal class DeleteDepositorCommandValidator : AbstractValidator<DeleteDepositorCommand>
{
    public DeleteDepositorCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorErrors.ValidationCodeIsRequired));
    }
}