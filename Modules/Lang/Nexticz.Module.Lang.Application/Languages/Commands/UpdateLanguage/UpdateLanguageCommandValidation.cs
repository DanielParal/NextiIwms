using FluentValidation;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Lang.Application.Languages.Commands.UpdateLanguage;

public class UpdateLanguageCommandValidation : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageCommandValidation()
    {
        RuleFor(x => x.UpdateLanguageRequest.IsActive)
            .NotNull()
            .WithState(x => new CustomErrorState(LanguageErrors.UpdateLanguageError));
    }
}