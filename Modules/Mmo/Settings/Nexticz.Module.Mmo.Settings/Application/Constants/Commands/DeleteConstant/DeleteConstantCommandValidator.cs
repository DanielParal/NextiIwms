using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.DeleteConstant;

internal class DeleteConstantCommandValidator : AbstractValidator<DeleteConstantCommand>
{
    public DeleteConstantCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ConstantErrors.ValidationKeyIsRequired));
    }
}