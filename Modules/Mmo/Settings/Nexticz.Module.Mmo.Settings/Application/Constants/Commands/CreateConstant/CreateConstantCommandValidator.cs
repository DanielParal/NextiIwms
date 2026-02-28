using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.CreateConstant;

internal class CreateConstantCommandValidator : AbstractValidator<CreateConstantCommand>
{
    public CreateConstantCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ConstantErrors.ValidationKeyIsRequired));
        
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ConstantErrors.ValidationValueIsRequired));
    }
}