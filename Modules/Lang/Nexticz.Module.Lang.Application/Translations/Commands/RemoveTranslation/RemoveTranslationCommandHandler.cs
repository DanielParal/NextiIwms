using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Module.Lang.Application.Common.Interfaces;

namespace Nexticz.Module.Lang.Application.Translations.Commands.RemoveTranslation;

public class RemoveTranslationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveTranslationCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(RemoveTranslationCommand command, CancellationToken cancellationToken)
    {
        var translation = await unitOfWork.TranslationRepository.GetTranslationByIdAsync(command.Id, cancellationToken);

        if (translation is null)
        {
            return TranslationErrors.TranslationWithIdDoesnotExist;
        }
        
        unitOfWork.Remove(translation);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}