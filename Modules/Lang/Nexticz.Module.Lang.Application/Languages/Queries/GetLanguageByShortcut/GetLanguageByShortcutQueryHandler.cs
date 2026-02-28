using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Module.Lang.Application.Common.Interfaces;

namespace Nexticz.Module.Lang.Application.Languages.Queries.GetLanguageByShortcut;

public class GetLanguageByShortcutQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLanguageByShortcutQuery, ErrorOr<LanguageResponse>>
{
    public async Task<ErrorOr<LanguageResponse>> Handle(GetLanguageByShortcutQuery query, CancellationToken cancellationToken)
    {
        var languageResponse = await unitOfWork.LanguageRepository.GetLanguageResponseByShortcutAsync(query.Shortcut, cancellationToken);

        if (languageResponse is null)
        {
            return LanguageErrors.LanguageWithShortcutDoesnotExist;
        }

        return languageResponse;
    }
}