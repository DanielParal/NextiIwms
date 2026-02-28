using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.UploadUserSignature;

internal class UploadUserSignatureCommandValidator : AbstractValidator<UploadUserSignatureCommand>
{
    public UploadUserSignatureCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithState(x => new CustomErrorState(UserErrors.ValidationUserNameIsRequired));
    }
}