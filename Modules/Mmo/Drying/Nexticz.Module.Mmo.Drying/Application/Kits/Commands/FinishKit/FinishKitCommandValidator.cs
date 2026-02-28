using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.FinishKit;

internal class FinishKitCommandValidator : AbstractValidator<FinishKitCommand>
{
    public FinishKitCommandValidator()
    {
        RuleFor(x => x.KitId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationKitIdIsRequired));

    }
}