using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.EnterLine;

internal class EnterLineCommandValidator : AbstractValidator<EnterLineCommand>
{
    public EnterLineCommandValidator()
    {
        RuleFor(x => x.LineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(LastEnteredWorkerOnLineErrors.ValidationLineCodeIsRequired));
    }
}