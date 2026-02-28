using ErrorOr;
using MediatR;

namespace Nexticz.Module.Lang.Application.Translations.Commands.RemoveTranslation;

public class RemoveTranslationCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}