using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Application.Common.Interfaces;
using Nexticz.Module.Lang.Application.Translations.Common.Models;

namespace Nexticz.Module.Lang.Application.Translations.Commands.CreateTranslations;

public class CreateTranslationsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTranslationsCommand, ErrorOr<CreateTranslationsResponse>>
{
    public async Task<ErrorOr<CreateTranslationsResponse>> Handle(CreateTranslationsCommand command, CancellationToken cancellationToken)
    {
        var updatedSlug= "";
        foreach (var createTranslationsItem in command.CreateTranslationsRequest.Items)
        {
            GetTranslationInfoFromSlug(createTranslationsItem.Slug, out var feature, out var module,
                out var component, out var name);

            updatedSlug = createTranslationsItem.Slug;
            
            var translation = new Translation
            {
                Feature = feature,
                Module = module,
                Component = component,
                Name = name,
                Slug = createTranslationsItem.Slug,
                Value = createTranslationsItem.Value,
                LanguageShortcut = command.CreateTranslationsRequest.LanguageShortcut
            };

            var storedTranslation = await unitOfWork.TranslationRepository.GetTranslationBySlugAndLanguageShortcutAsync(
                createTranslationsItem.Slug, command.CreateTranslationsRequest.LanguageShortcut, cancellationToken);
            if (storedTranslation is null)
            {
                unitOfWork.Add(translation);   
            }
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        
        var translationFilrteringparams = new TranslationsFilteringParams
        {
            Slug = updatedSlug,
            LanguageShortcut = EnumHelper.LanguageShortcutEnum.Cs
        };
        var componentTranslations =
            await unitOfWork.TranslationRepository.ListFilteredTranslationsAsync(translationFilrteringparams,
                cancellationToken);

        var componentTranslactions = new CreateTranslationsResponse()
        {
            Properties = new Dictionary<string, string>()
        };
        foreach (var translation in componentTranslations.data.OfType<TranslationResponse>())
        {
            componentTranslactions.Properties.Add(translation.Name, translation.Value);
        }
        
        return componentTranslactions;
    }

    private static void GetTranslationInfoFromSlug(string slug, out string feature, out string module, out string component, out string name)
    {
        var splitedComponentSlug = slug.Split("-");
        module = splitedComponentSlug[0];
        feature = splitedComponentSlug[1];
        component = splitedComponentSlug[2];
        name = splitedComponentSlug[3];
    }
}