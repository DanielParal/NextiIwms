using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.LeaveLine;

internal class LeaveLineCommandValidator : AbstractValidator<LeaveLineCommand>
{
    public LeaveLineCommandValidator()
    {
        RuleFor(x => x.LineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(LastEnteredWorkerOnLineErrors.ValidationLineCodeIsRequired));
    }
}