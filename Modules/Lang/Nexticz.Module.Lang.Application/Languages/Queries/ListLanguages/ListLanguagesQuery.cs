using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Lang.Application.Languages.Common.Models;


namespace Nexticz.Module.Lang.Application.Languages.Queries.ListLanguages;

public class ListLanguagesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required LanguagesFilteringParams FilteringParams { get; set; }
}