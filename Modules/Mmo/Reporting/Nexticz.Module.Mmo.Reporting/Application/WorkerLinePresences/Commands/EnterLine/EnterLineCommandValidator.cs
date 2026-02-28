using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Reporting.Application.WorkerLinePresences.Commands.EnterLine;

internal class EnterLineCommandValidator : AbstractValidator<EnterLineCommand>
{
    public EnterLineCommandValidator()
    {
        RuleFor(x => x.LineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WorkerLinePresenceErrors.ValidationLineCodeIsRequired));
    }
}