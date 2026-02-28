using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserSignature;

internal class DeleteUserSignatureCommandValidator : AbstractValidator<DeleteUserSignatureCommand>
{
    public DeleteUserSignatureCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithState(x => new CustomErrorState(UserErrors.ValidationUserNameIsRequired));
    }
}