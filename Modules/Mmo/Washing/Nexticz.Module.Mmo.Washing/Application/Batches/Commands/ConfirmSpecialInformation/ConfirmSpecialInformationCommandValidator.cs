using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.ConfirmSpecialInformation;

internal class ConfirmSpecialInformationCommandValidator : AbstractValidator<ConfirmSpecialInformationCommand>
{
    public ConfirmSpecialInformationCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(BatchErrors.ValidationIdIsRequired));
    }
}