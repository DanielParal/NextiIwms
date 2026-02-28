using ErrorOr;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants;

internal abstract class ConstantErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-constantService-";
    
    public static Error ValidationKeyIsRequired => Error.Validation(
        ComponentSlug + "ValidationKeyIsRequired",
        "Jméno konfigurace není nastaveno."
    );
    
    public static Error ValidationValueIsRequired => Error.Validation(
        ComponentSlug + "ValidationValueIsRequired",
        "Hodnota konfigurace není nastavena."
    );
    
    public static Error ValidationKeyAlreadyExists(string key) => Error.Validation(
        ComponentSlug + "ValidationKeyAlreadyExists",
        $"Konstanta s id: '{key}' již existuje."
    );
    
    public static Error ValidationValueIsNotCorrectType(string value, ConstantType type) => Error.Validation(
        ComponentSlug + "ValidationValueIsNotCorrectType",
        $"Hodnota: '{value}' není typu: '{type.ToString()}'."
    );
    
    public static Error KeyDoesNotExist => Error.NotFound(
        ComponentSlug + "KeyDoesNotExist",
        "Konstanta s daným Id neexistuje.");
}