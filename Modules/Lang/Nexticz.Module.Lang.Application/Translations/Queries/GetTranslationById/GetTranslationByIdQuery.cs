using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Translations;

namespace Nexticz.Module.Lang.Application.Translations.Queries.GetTranslationById;

public class GetTranslationByIdQuery : IRequest<ErrorOr<TranslationResponse>>
{
    public required Guid Id { get; set; }
}