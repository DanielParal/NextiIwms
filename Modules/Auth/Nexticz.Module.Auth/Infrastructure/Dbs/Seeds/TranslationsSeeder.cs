using ErrorOr;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Contracts;

namespace Nexticz.Module.Auth.Infrastructure.Dbs.Seeds;

public class TranslationsSeeder()
{
    public async Task SeedAsync()
    {
        // TODO: this needs to be implemented
        
        // var logger = _serviceProvider.GetRequiredService<ILogger<TranslationsSeed>>();
        // var langApiService = _serviceProvider.GetRequiredService<ILangApiService>();
        // var healthService = _serviceProvider.GetRequiredService<IHealtService>();
        // var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        // var mediatr = _serviceProvider.GetRequiredService<ISender>();
        // var errorsToSeed = new List<Error>();
        //
        // errorsToSeed.AddRange(GetErrorsFromClassProperties(typeof(AuthenticationErrors)));
        // errorsToSeed.AddRange(GetErrorsFromClassProperties(typeof(Errors.Common)));
        //
        // var createTranslationsRequest = new CreateTranslationsRequest
        // {
        //     Items = GetCreateTranslationsItems(errorsToSeed),
        //     LanguageShortcut = EnumHelper.LanguageShortcutEnum.Cs
        // };
        //
        // var thirdPartyApisKeysSettings = configuration.GetSection(nameof(ThirdPartyApisKeys)).Get<ThirdPartyApisKeys>() 
        //             ?? throw new InvalidOperationException("ThirdPartyApisKeys Settings not found.");
        // var query = new GetAccessTokenForApiKeyQuery(thirdPartyApisKeysSettings.MagicLang);
        // var result = await mediatr.Send(query);
        //
        // if (result.IsError)
        // {
        //     Console.WriteLine(result.FirstError.Description);
        // }
        // var jwtHeader = "Bearer " + result.Value;
        //
        // var i = 0;
        // while (true)
        // {
        //     try
        //     {
        //         logger.LogInformation("Cekam na spuštění API, abych mohl seedovat");
        //         i++;
        //         await Task.Delay(1000);
        //         await healthService.CheckHealth();
        //         break;
        //     }
        //     catch
        //     {
        //         if (i > 5)
        //         {
        //             logger.LogWarning("API se nespustilo");
        //         }
        //     }
        // }
        // await langApiService.CreateTranslations(jwtHeader, createTranslationsRequest);
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