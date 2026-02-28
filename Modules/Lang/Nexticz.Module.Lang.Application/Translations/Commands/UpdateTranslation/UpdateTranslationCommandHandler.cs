using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Module.Lang.Application.Common.Interfaces;

namespace Nexticz.Module.Lang.Application.Translations.Commands.UpdateTranslation;

public class UpdateTranslationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTranslationCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateTranslationCommand command, CancellationToken cancellationToken)
    {
        var translation = await unitOfWork.TranslationRepository.GetTranslationByIdAsync(command.Id, cancellationToken);

        if (translation is null)
        {
            return TranslationErrors.TranslationWithIdDoesnotExist;
        }

        translation.Value = command.UpdateTranslationRequest.Value;
        
        unitOfWork.Update(translation);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}