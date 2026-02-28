using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UploadKitInstruction;

internal class UploadKitInstructionCommandValidator : AbstractValidator<UploadKitInstructionCommand>
{
    public UploadKitInstructionCommandValidator()
    {
        RuleFor(x => x.KitCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationCodeIsRequired));
    }
}