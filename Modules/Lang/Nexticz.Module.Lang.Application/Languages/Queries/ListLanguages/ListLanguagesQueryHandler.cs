using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Lang.Application.Common.Interfaces;


namespace Nexticz.Module.Lang.Application.Languages.Queries.ListLanguages;

public class ListLanguagesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<ListLanguagesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(ListLanguagesQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.LanguageRepository.ListFilteredLanguagesAsync(query.FilteringParams, cancellationToken);
    }
}