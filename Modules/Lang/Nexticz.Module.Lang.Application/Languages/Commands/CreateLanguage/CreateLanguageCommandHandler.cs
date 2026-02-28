using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Module.Lang.Application.Common.Interfaces;

namespace Nexticz.Module.Lang.Application.Languages.Commands.CreateLanguage;

public class CreateLanguageCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateLanguageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
    {
        unitOfWork.Add(command.Language);
        var result = await unitOfWork.CompleteAsync(cancellationToken);

        if (!result)
        {
            return LanguageErrors.CreateLanguageError;
        }

        return Result.Success;
    }
}