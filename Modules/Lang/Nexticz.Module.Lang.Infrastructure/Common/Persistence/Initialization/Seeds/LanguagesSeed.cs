using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Lang.Application.Languages.Commands.CreateLanguage;
using Nexticz.Module.Lang.Application.Languages.Queries.GetLanguageByShortcut;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Infrastructure.Common.Persistence.Initialization.Seeds;

public class LanguagesSeed
{
    private readonly IServiceProvider _serviceProvider;

    public LanguagesSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
        var mediatr = _serviceProvider.GetRequiredService<ISender>();
        var logger = _serviceProvider.GetRequiredService<ILogger<CreateLanguageCommandHandler>>();

        var seedLanguages = new List<Language>
        {
            new() { Name = "Czech", Shortcut = EnumHelper.LanguageShortcutEnum.Cs, IsActive = false },
            new() { Name = "English", Shortcut = EnumHelper.LanguageShortcutEnum.En, IsActive = false },
            new() { Name = "German", Shortcut = EnumHelper.LanguageShortcutEnum.De, IsActive = false },
            new() { Name = "Spanish", Shortcut = EnumHelper.LanguageShortcutEnum.Es, IsActive = false },
            new() { Name = "French", Shortcut = EnumHelper.LanguageShortcutEnum.Fr, IsActive = false },
            new() { Name = "Hungarian", Shortcut = EnumHelper.LanguageShortcutEnum.Hu, IsActive = false },
            new() { Name = "Italian", Shortcut = EnumHelper.LanguageShortcutEnum.It, IsActive = false },
            new() { Name = "Lithuanian", Shortcut = EnumHelper.LanguageShortcutEnum.Lt, IsActive = false },
            new() { Name = "Polish", Shortcut = EnumHelper.LanguageShortcutEnum.Pl, IsActive = false },
            new() { Name = "Romanian", Shortcut = EnumHelper.LanguageShortcutEnum.Ro, IsActive = false },
            new() { Name = "Russian", Shortcut = EnumHelper.LanguageShortcutEnum.Ru, IsActive = false },
            new() { Name = "Slovenian", Shortcut = EnumHelper.LanguageShortcutEnum.Sl, IsActive = false }
        };
        
        foreach (var seedLanguage in seedLanguages)
        {
            var query = new GetLanguageByShortcutQuery { Shortcut = seedLanguage.Shortcut };
            var queryResult = await mediatr.Send(query);

            if (queryResult.Value is not null)
            {
                continue;
            }
            
            if (queryResult.IsError)
            {
                logger.LogError(LanguageErrors.LanguageWithShortcutDoesnotExist.Description);
            }   
            
            var command = new CreateLanguageCommand { Language = seedLanguage };
            var result = await mediatr.Send(command);
            
            if (result.IsError)
            {
                logger.LogError(result.FirstError.Description);
            }   
        }
    }
}