using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings;

internal abstract class PackagingErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-packagingService-";
    
    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationCodeIsRequired",
        "Unikátní kód obalu není vyplněn"
    );
    
    public static Error ValidationDepositorCodeIsEmpty => Error.Validation(
        ComponentSlug + "validationDepositorCodeIsRequired",
        "Ukladatel není vyplněn"
    );
    
    public static Error ValidationDepositorDoesNotExist => Error.Validation(
        ComponentSlug + "validationDepositorDoesNotExist",
        "Zadaný ukladatel neexistuje"
    );
    
    public static Error ValidationPackagingTypeIsEmpty => Error.Validation(
        ComponentSlug + "validationPackagingTypeIsRequired",
        "Zadaný typ obalu není vyplněn"
    );
    
    public static Error ValidationPackagingTypeDoesNotExist => Error.Validation(
        ComponentSlug + "validationPackagingTypeDoesNotExist",
        "Zadaný typ obalu neexistuje"
    );
    
    public static Error ValidationPackagingCirculationIsEmpty => Error.Validation(
        ComponentSlug + "validationPackagingCirculationIsRequired",
        "Oběhovost obalu není vyplněna"
    );
    
    public static Error ValidationPackagingCirculationDoesNotExist => Error.Validation(
        ComponentSlug + "validationPackagingCirculationDoesNotExist",
        "Oběhovost obalu neexistuje"
    );
    
    public static Error ValidationCustomerNumberIsEmpty => Error.Validation(
        ComponentSlug + "validationPackagingCustomerNumberIsRequired",
        "Zákaznické číslo není vyplněno"
    );
    
    public static Error ValidationPackagingDepthGraterThan0 => Error.Validation(
        ComponentSlug + "validationPackagingDepthGraterThan0",
        "Hloubka musí být větší než 0"
    );
    
    public static Error ValidationPackagingWidthGraterThan0 => Error.Validation(
        ComponentSlug + "validationPackagingWidthGraterThan0",
        "Šířka musí být větší než 0"
    );
    
    public static Error ValidationPackagingHeightGraterThan0 => Error.Validation(
        ComponentSlug + "validationPackagingHeightGraterThan0",
        "Výška musí být větší než 0"
    );
    
    public static Error ValidationPackagingWeightGraterThan0 => Error.Validation(
        ComponentSlug + "validationPackagingWeightGraterThan0",
        "Váha musí být větší než 0"
    );
    
    public static Error ValidationCombinationDepositorAndNumberExists => Error.Validation(
        ComponentSlug + "validationCombinationDepositorAndNumberExists",
        "Kombinace ukladatele a čísla balení již existuje"
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Obal s tímto kódem neexistuje");
    
    public static Error ValidationPackagingIsUsedInKits(string kitCodes) => Error.Validation(
        ComponentSlug + "validationPackagingIsUsedInKits",
        $"Obal je stále přiřazen v existujících kitech: {kitCodes}");
    
    public static Error ValidationPackagingSpeedsCountMismatch(string washingMachineCodes) => Error.Validation(
        ComponentSlug + "validationPackagingSpeedsCountMismatch",
        $"Počet rychlostí se neshoduje s počtem myček. Zkontrolujte nastavené rychlosti. Musíte nastavit rychlost pro tyto myčky: {washingMachineCodes}"
    );
    
    public static Error ValidationWashingMachineSpeedsIsRequired => Error.Validation(
        ComponentSlug + "validationWashingMachineSpeedsIsRequired",
        "Rychlosti myček nejsou vyplněny."
    );
    
    
}