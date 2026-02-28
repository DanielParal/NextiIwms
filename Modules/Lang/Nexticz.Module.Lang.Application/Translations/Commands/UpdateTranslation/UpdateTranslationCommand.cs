using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Translations;

namespace Nexticz.Module.Lang.Application.Translations.Commands.UpdateTranslation;

public class UpdateTranslationCommand : IRequest<ErrorOr<Updated>>
{
    public required UpdateTranslationRequest UpdateTranslationRequest { get; set; }
    public required Guid Id { get; set; }
}