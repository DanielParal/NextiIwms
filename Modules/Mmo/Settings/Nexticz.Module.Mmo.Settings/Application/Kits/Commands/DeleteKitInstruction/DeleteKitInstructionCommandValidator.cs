using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.DeleteKitInstruction;

internal class DeleteKitInstructionCommandValidator : AbstractValidator<DeleteKitInstructionCommand>
{
    public DeleteKitInstructionCommandValidator()
    {
        RuleFor(x => x.KitCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationCodeIsRequired));
    }
}