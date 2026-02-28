using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Lang.Application.Common.Interfaces;
using Nexticz.Module.Lang.Application.Translations.Common.Models;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Lang.Infrastructure.Common.Persistence;


namespace Nexticz.Module.Lang.Infrastructure.Translations.Persistance;

public class TranslationsRepository(DataContext context) : ITranslationsRepository
{
    public async Task<Translation?> GetTranslationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Translations
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<TranslationResponse?> GetTranslationResponseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Translations
            .Where(x => x.Id == id)
            .Select(x => new TranslationResponse
            {
                Id = x.Id,
                Module = x.Module,
                Feature = x.Feature,
                Component = x.Component,
                Name = x.Name,
                Slug = x.Slug,
                Value = x.Value,
                LanguageShortcut = x.LanguageShortcut,
                Created = x.Created.ToLocalTime(),
                Updated = x.Updated == null ? null : x.Updated.Value.ToLocalTime(),
                UpdatedWith = x.UpdatedWith
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TranslationResponse>> GetTranslationsBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await context.Translations
            .Where(x => x.Slug == slug)
            .Select(x => new TranslationResponse
            {
                Id = x.Id,
                Module = x.Module,
                Feature = x.Feature,
                Component = x.Component,
                Name = x.Name,
                Slug = x.Slug,
                Value = x.Value,
                LanguageShortcut = x.LanguageShortcut,
                Created = x.Created.ToLocalTime(),
                Updated = x.Updated == null ? null : x.Updated.Value.ToLocalTime(),
                UpdatedWith = x.UpdatedWith
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TranslationResponse?> GetTranslationBySlugAndLanguageShortcutAsync(string slug, EnumHelper.LanguageShortcutEnum shortcut,
        CancellationToken cancellationToken)
    {
        return await context.Translations
            .Where(x => x.Slug == slug)
            .Where(x => x.LanguageShortcut == shortcut)
            .Select(x => new TranslationResponse
            {
                Id = x.Id,
                Module = x.Module,
                Feature = x.Feature,
                Component = x.Component,
                Name = x.Name,
                Slug = x.Slug,
                Value = x.Value,
                LanguageShortcut = x.LanguageShortcut,
                Created = x.Created,
                Updated = x.Updated,
                UpdatedWith = x.UpdatedWith
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FilteredResult> ListFilteredTranslationsAsync(TranslationsFilteringParams filteringParams,
        CancellationToken cancellationToken)
    {
        var query = context.Translations
            .Where(x => filteringParams.Slug == null || x.Slug == filteringParams.Slug)
            .Where(x => filteringParams.Module == null || x.Module == filteringParams.Module)
            .Where(x => filteringParams.Feature == null || x.Feature == filteringParams.Feature)
            .Where(x => filteringParams.Component == null || x.Component == filteringParams.Component)
            .Where(x => filteringParams.LanguageShortcut == null || x.LanguageShortcut == filteringParams.LanguageShortcut)
            .Select(x => new TranslationResponse
            {
                Id = x.Id,
                Module = x.Module,
                Feature = x.Feature,
                Component = x.Component,
                Name = x.Name,
                Slug = x.Slug,
                Value = x.Value,
                LanguageShortcut = x.LanguageShortcut,
                Created = x.Created,
                Updated = x.Updated,
                UpdatedWith = x.UpdatedWith
            });

        var loadOption = FilteringHelper.CreateLoadOptionsFromFilteringParams(filteringParams);
        var loadResult = await DataSourceLoader.LoadAsync(query, loadOption, cancellationToken);

        return loadResult.MapToFilteredResult();
    }
}