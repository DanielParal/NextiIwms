using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Application.Languages.Common.Models;


namespace Nexticz.Module.Lang.Application.Common.Interfaces;

public interface ILanguagesRepository
{
    Task<Language?> GetLanguageByShortcutAsync(EnumHelper.LanguageShortcutEnum shortcut, CancellationToken cancellationToken);
    Task<LanguageResponse?> GetLanguageResponseByShortcutAsync(EnumHelper.LanguageShortcutEnum shortcut, CancellationToken cancellationToken);
    Task<FilteredResult> ListFilteredLanguagesAsync(LanguagesFilteringParams filteringParams, CancellationToken cancellationToken);
}