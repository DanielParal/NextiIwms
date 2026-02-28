using ErrorOr;
using MediatR;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Application.Languages.Queries.GetLanguageByShortcut;

public class GetLanguageByShortcutQuery : IRequest<ErrorOr<LanguageResponse>>
{
    public required EnumHelper.LanguageShortcutEnum Shortcut { get; set; }
}