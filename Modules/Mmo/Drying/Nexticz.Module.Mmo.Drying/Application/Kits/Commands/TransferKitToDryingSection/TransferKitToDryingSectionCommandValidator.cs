using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Drying.Application.Kits.Commands.TransferKitToDryingSection;

internal class TransferKitToDryingSectionCommandValidator : AbstractValidator<TransferKitToDryingSectionCommand>
{
    public TransferKitToDryingSectionCommandValidator()
    {
        RuleFor(x => x.KitId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationKitIdIsRequired));

    }
}