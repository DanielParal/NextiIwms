using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Lang.Application.Common.Interfaces;
using Nexticz.Module.Lang.Application.Languages.Common.Models;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Lang.Infrastructure.Languages.Persistance;

public class LanguagesRepository(DataContext context) : ILanguagesRepository
{
    public async Task<Language?> GetLanguageByShortcutAsync(EnumHelper.LanguageShortcutEnum shortcut,  CancellationToken cancellationToken)
    {
        return await context.Languages
            .Where(x => x.Shortcut == shortcut)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<LanguageResponse?> GetLanguageResponseByShortcutAsync(EnumHelper.LanguageShortcutEnum shortcut,  CancellationToken cancellationToken)
    {
        return await context.Languages
            .Where(x => x.Shortcut == shortcut)
            .Select(x => new LanguageResponse{Name = x.Name, Shortcut = x.Shortcut, IsActive = x.IsActive})
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<FilteredResult> ListFilteredLanguagesAsync(LanguagesFilteringParams filteringParams, CancellationToken cancellationToken)
    {
        var query = context.Languages
            .Select(x => new LanguageResponse{Name = x.Name, Shortcut = x.Shortcut, IsActive = x.IsActive});

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}