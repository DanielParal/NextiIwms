using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Application.Translations.Common.Models;


namespace Nexticz.Module.Lang.Application.Common.Interfaces;

public interface ITranslationsRepository
{
    Task<Translation?> GetTranslationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TranslationResponse?> GetTranslationResponseByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TranslationResponse>> GetTranslationsBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<TranslationResponse?> GetTranslationBySlugAndLanguageShortcutAsync(string slug, EnumHelper.LanguageShortcutEnum shortcut, CancellationToken cancellationToken = default);
    Task<FilteredResult> ListFilteredTranslationsAsync(TranslationsFilteringParams filteringParams,
        CancellationToken cancellationToken);
}