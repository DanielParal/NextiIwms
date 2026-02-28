using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Lang.Application.Common.Interfaces;


namespace Nexticz.Module.Lang.Application.Translations.Queries.ListTranslations;

public class ListTranslationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<ListTranslationsQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(ListTranslationsQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.TranslationRepository.ListFilteredTranslationsAsync(query.FilteringParams, cancellationToken);
    }
}