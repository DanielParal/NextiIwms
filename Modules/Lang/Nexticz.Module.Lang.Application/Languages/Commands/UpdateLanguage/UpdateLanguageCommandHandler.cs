using ErrorOr;
using Google.Cloud.Translation.V2;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Application.Common.Interfaces;
using Nexticz.Module.Lang.Application.Languages.Configurations;
using Nexticz.Module.Lang.Application.Translations.Common.Models;

namespace Nexticz.Module.Lang.Application.Languages.Commands.UpdateLanguage;

public class UpdateLanguageCommandHandler(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<UpdateLanguageCommandHandler> logger) : IRequestHandler<UpdateLanguageCommand, ErrorOr<Updated>>
{
    private readonly GoogleApisSettings _googleApisSettings = configuration.GetSection(nameof(GoogleApisSettings)).Get<GoogleApisSettings>() 
                                                              ?? throw new InvalidOperationException("Google Apis Settings not found.");
    
    public async Task<ErrorOr<Updated>> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        var language = await unitOfWork.LanguageRepository.GetLanguageByShortcutAsync(command.Shortcut, cancellationToken);

        if (language is null)
        {
            return LanguageErrors.LanguageWithShortcutDoesnotExist;
        }

        language.IsActive = command.UpdateLanguageRequest.IsActive;
        
        unitOfWork.Update(language);
        var result = await unitOfWork.CompleteAsync(cancellationToken);

        if (!result)
        {
            return LanguageErrors.UpdateLanguageError;
        }
        
        var translationClient = TranslationClient.CreateFromApiKey(_googleApisSettings.LangApiKey);
    
        var loadParameters = new TranslationsFilteringParams();
        var translates = await unitOfWork.TranslationRepository.ListFilteredTranslationsAsync(loadParameters, cancellationToken);
    
        var filteredGroups = translates.data.OfType<TranslationResponse>()
            .GroupBy(translation => translation.Slug)
            .Where(group => group.All(translation => translation.LanguageShortcut != command.Shortcut))
            .ToList();
        var newItemsToTranslate = filteredGroups
            .SelectMany(x => x)
            .Where(x => x.LanguageShortcut == EnumHelper.LanguageShortcutEnum.Cs)
            .ToList();
    
        foreach (var translation in newItemsToTranslate)
        {
            TranslationResult? googleTranslate = null;
            try
            {
                googleTranslate = await translationClient.TranslateTextAsync(translation.Value, command.Shortcut.ToString(),
                    EnumHelper.LanguageShortcutEnum.Cs.ToString(), null, cancellationToken);
            }
            catch (Exception)
            {
                logger.LogError(TranslationErrors.CreateTranslationError.Description);
            }
    
            var item = new Translation
            {
                Module = translation.Module,
                Feature = translation.Feature,
                Component = translation.Component,
                Name = translation.Name,
                Slug = translation.Slug,
                Value = googleTranslate != null ? googleTranslate.TranslatedText : translation.Value,
                LanguageShortcut = command.Shortcut
            };
    
            unitOfWork.Add(item);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        

        return Result.Updated;
    }
}