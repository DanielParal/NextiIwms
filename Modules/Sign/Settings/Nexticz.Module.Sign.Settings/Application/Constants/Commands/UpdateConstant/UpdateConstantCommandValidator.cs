using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Commands.UpdateConstant;

internal class UpdateConstantCommandValidator : AbstractValidator<UpdateConstantCommand>
{
    public UpdateConstantCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ConstantErrors.ValidationKeyIsRequired));
    }
}