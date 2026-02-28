using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.DeleteSigningDevice;

internal class DeleteSigningDeviceCommandValidator : AbstractValidator<DeleteSigningDeviceCommand>
{
    public DeleteSigningDeviceCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SigningDeviceErrors.ValidationCodeIsRequired));
    }
}