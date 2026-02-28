using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.Kits;

internal abstract class KitErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-kitService-";
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Kit s tímto kódem neexistuje");
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Unikátní kód kitu není vyplněn"
    );
    
    public static Error ValidationKitDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationKitDoesNotExist",
        "Kit s tímto kódem neexistuje."
    );
    
    public static Error ValidationKitNumberIsEmpty => Error.Validation(
        ComponentSlug + "validationKitNumberIsRequired",
        "Číslo kitu není vyplněno"
    );
    
    public static Error ValidationDepositorCodeIsEmpty => Error.Validation(
        ComponentSlug + "validationDepositorCodeIsRequired",
        "Ukladatel není vyplněn"
    );
    
    public static Error ValidationDepositorDoesNotExist => Error.Validation(
        ComponentSlug + "validationDepositorDoesNotExist",
        "Zadaný ukladatel neexistuje"
    );
    
    public static Error ValidationKitTypeIsEmpty => Error.Validation(
        ComponentSlug + "validationKitTypeIsRequired",
        "Zadaný typ kitu není vyplněn"
    );
    
    public static Error ValidationKitTypeDoesNotExist => Error.Validation(
        ComponentSlug + "validationKitTypeDoesNotExist",
        "Zadaný typ kitu neexistuje"
    );
    
    public static Error ValidationKitSapDefinitionDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationKitSapDefinitionDoesNotExist",
        "Definice SAPu kitu neexistuje"
    );
    
    public static Error ValidationManufactureIsEmpty => Error.Validation(
        ComponentSlug + "validationManufactureIsRequired",
        "Výroba není vyplněna"
    );
    
    public static Error ValidationManufactureDoesNotExist => Error.Validation(
        ComponentSlug + "validationManufactureDoesNotExist",
        "Výroba neexistuje"
    );
    
    public static Error ValidationDefiningPackagingCodeIsEmpty => Error.Validation(
        ComponentSlug + "validationDefiningPackagingCodeIsEmpty",
        "Číslo určující komponenty není vyplněno"
    );
    
    public static Error ValidationDryingTimeGraterThan0 => Error.Validation(
        ComponentSlug + "validationDryingTimeGraterThan0",
        "Čas chaldnutí musí být větší než 0"
    );
    
    public static Error ValidationCombinationDepositorAndKitNumberExist => Error.Validation(
        ComponentSlug + "combinationDepositorAndKitNumberExists",
        "Kombinace ukladatele a čísla kitu již existuje");
    
    public static Error ValidationMissingPackagingCodes(string codes) => Error.Validation(
        ComponentSlug + "validationMissingPackagingCodes",
        $"Tyto kódy obalů nejsou nalezeny mezi obaly: {codes}");
    
    public static Error ValidationDefiningPackagingCodeNotIncludedInPackagingCodes => Error.Validation(
        ComponentSlug + "validationDefiningPackagingCodeNotIncludedInPackagingCodes",
        "Kód určujícího obalu není nalezen mezi seznamem obalů na kitu");
    
    public static Error ValidationPackagingCodeQuantitiesIsRequired => Error.Validation(
        ComponentSlug + "validationPackagingCodeQuantitiesIsRequired",
        "Musíte vyplnit nějaké obaly pro tento kit");
    
    public static Error ValidationFileIsNotPdf => Error.Validation(
        ComponentSlug + "ValidationFileIsNotPdf",
        $"Soubor není pdf."
    );
    
    public static Error NotFoundKitInstructionFile => Error.NotFound(
        ComponentSlug + "ValidationKitInstructionFileNotFound",
        "Balící předpis pro tento kit nebyl nalezen."
    );
    
    public static Error ValidationOverlappingSpecialInformations => Error.NotFound(
        ComponentSlug + "ValidationOverlappingSpecialInformations",
        "Speciální informace se překrývají se svojí platností."
    );
    
    public static Error ValidationNonExistingSpecialInformations => Error.NotFound(
        ComponentSlug + "ValidationNonExistingSpecialInformations",
        "Speciální informace obsahují ID, která nejsou v databázi."
    );
    
    
}