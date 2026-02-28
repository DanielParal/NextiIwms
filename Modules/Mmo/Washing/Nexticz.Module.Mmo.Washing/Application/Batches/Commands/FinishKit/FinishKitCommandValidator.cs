using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.FinishKit;

internal class FinishKitCommandValidator : AbstractValidator<FinishKitCommand>
{
    public FinishKitCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(BatchErrors.ValidationIdIsRequired));
    }
}