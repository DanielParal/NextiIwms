using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Lang.Application.Translations.Common.Models;


namespace Nexticz.Module.Lang.Application.Translations.Queries.ListTranslations;

public class ListTranslationsQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required TranslationsFilteringParams FilteringParams { get; set; }
}