using ErrorOr;
using FluentValidation;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Manufactures;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.DeleteDepositor;

internal class DeleteDepositorCommandValidator : AbstractValidator<DeleteDepositorCommand>
{
    public DeleteDepositorCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DepositorErrors.ValidationCodeIsRequired));
    }
}