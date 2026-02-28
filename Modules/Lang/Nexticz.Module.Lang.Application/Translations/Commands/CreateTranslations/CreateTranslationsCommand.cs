using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Translations;

namespace Nexticz.Module.Lang.Application.Translations.Commands.CreateTranslations;

public class CreateTranslationsCommand : IRequest<ErrorOr<CreateTranslationsResponse>>
{
    public required CreateTranslationsRequest CreateTranslationsRequest { get; set; }
}