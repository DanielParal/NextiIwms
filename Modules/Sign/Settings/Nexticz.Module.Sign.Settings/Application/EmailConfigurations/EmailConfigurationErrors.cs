using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations;

internal abstract class EmailConfigurationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-emailConfigurationService-";
    
    public static Error CodesCombinationNotFound => Error.NotFound(
        ComponentSlug + "CodesCombinationNotFound",
        "Kombinace kódů neexistuje.");
    
    public static Error LoadingConfigurationNotFound => Error.NotFound(
        ComponentSlug + "LoadingConfigurationNotFound",
        "Emailová konfigurace pro nakládkový list nenalezena.");
    
    public static Error IdNotFound => Error.NotFound(
        ComponentSlug + "IdNotFound",
        "Nastavení s tímto id neexistuje.");
    
    public static Error ValidationCombinationOfCodesAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationCombinationOfCodesAlreadyExists",
        "Kombinace kódů ukladatele, partnera a příjemce již existuje.");
    
    public static Error ValidationIdDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationIdDoesNotExist",
        "Nastavení s tímto id neexistuje.");
    
    public static Error ValidationNoEmailProvided => Error.Validation(
        ComponentSlug + "ValidationNoEmailProvided",
        "Musíte zadat e-mail.");
    
    public static Error ValidationInvalidEmail(string email) => Error.Validation(
        ComponentSlug + "ValidationInvalidEmail",
        $"Neplatný e-mail: {email}.");
        
}