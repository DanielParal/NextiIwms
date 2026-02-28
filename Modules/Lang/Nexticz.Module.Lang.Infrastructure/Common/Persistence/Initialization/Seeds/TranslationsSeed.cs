using ErrorOr;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Lang.Application.Translations.Commands.CreateTranslations;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Module.Lang.Domain.Languages;
using Nexticz.Module.Lang.Domain.Translations;
using Nexticz.Lib.Shared.Helpers;
using Errors = Nexticz.Lib.Shared.Errors.Errors;

namespace Nexticz.Module.Lang.Infrastructure.Common.Persistence.Initialization.Seeds;

public class TranslationsSeed
{
    private readonly IServiceProvider _serviceProvider;

    public TranslationsSeed(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunSeed()
    {
        var mediatr = _serviceProvider.GetRequiredService<ISender>();
        var logger = _serviceProvider.GetRequiredService<ILogger<CreateTranslationsCommandHandler>>();

        var errorsToSeed = new List<Error>();

        errorsToSeed.AddRange(GetErrorsFromClassProperties(typeof(TranslationErrors)));
        errorsToSeed.AddRange(GetErrorsFromClassProperties(typeof(LanguageErrors)));
        errorsToSeed.AddRange(GetErrorsFromClassProperties(typeof(Errors.Common)));

        var createTranslationsRequest = new CreateTranslationsRequest
        {
            Items = GetCreateTranslationsItems(errorsToSeed),
            LanguageShortcut = EnumHelper.LanguageShortcutEnum.Cs
        };

        var command = new CreateTranslationsCommand { CreateTranslationsRequest = createTranslationsRequest };
        var result = await mediatr.Send(command);

        if (result.IsError)
        {
            logger.LogError(result.FirstError.Description);
        }
    }
    
    private List<Error> GetErrorsFromClassProperties(Type errorClass)
    {
        var errors = new List<Error>();
        
        var sharedClassProperties = errorClass.GetProperties();
        
        foreach (var sharedClassProperty in sharedClassProperties)
        {
            if (sharedClassProperty.PropertyType == typeof(Error))
            {
                Error error = (Error)sharedClassProperty.GetValue(null)!;
                errors.Add(error);
            }
        }

        return errors;
    }

    private List<CreateTranslationsItem> GetCreateTranslationsItems(List<Error> errors)
    {
        return errors.Select(x => new CreateTranslationsItem { Slug = x.Code, Value = x.Description }).ToList();
    }
}