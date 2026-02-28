using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Application.Languages.Commands.UpdateLanguage;

public class UpdateLanguageCommand : IRequest<ErrorOr<Updated>>
{
    public required UpdateLanguageRequest UpdateLanguageRequest { get; set; }
    public required EnumHelper.LanguageShortcutEnum Shortcut { get; set; }
}