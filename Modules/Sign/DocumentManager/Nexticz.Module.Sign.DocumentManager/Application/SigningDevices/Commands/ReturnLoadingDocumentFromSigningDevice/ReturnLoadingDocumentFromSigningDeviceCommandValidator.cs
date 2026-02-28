using FluentValidation;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.ReturnLoadingDocumentFromSigningDevice;

internal class ReturnLoadingDocumentFromSigningDeviceCommandValidator : AbstractValidator<ReturnLoadingDocumentFromSigningDeviceCommand>
{
    public ReturnLoadingDocumentFromSigningDeviceCommandValidator()
    {
        RuleFor(x => x.SigningDeviceCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(SigningDeviceErrors.ValidationSigningDeviceCodeIsRequired));
    }
}