using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Module.Lang.Application.Common.Interfaces;

namespace Nexticz.Module.Lang.Application.Translations.Queries.GetTranslationById;

public class GetTranslationByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTranslationByIdQuery, ErrorOr<TranslationResponse>>
{
    public async Task<ErrorOr<TranslationResponse>> Handle(GetTranslationByIdQuery query, CancellationToken cancellationToken)
    {
        var translation =
            await unitOfWork.TranslationRepository.GetTranslationResponseByIdAsync(query.Id, cancellationToken);

        if (translation is null)
        {
            return TranslationErrors.TranslationWithIdDoesnotExist;
        }

        return translation;
    }
}