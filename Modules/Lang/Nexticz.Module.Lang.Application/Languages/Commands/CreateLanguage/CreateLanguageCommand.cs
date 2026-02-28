using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Domain.Languages;

namespace Nexticz.Module.Lang.Application.Languages.Commands.CreateLanguage;

public class CreateLanguageCommand : IRequest<ErrorOr<Success>>
{
    public required Language Language { get; set; }
}