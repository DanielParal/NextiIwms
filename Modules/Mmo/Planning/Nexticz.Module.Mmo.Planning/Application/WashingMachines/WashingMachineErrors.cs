using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines;

internal abstract class WashingMachineErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-planning-api-washingMachineService-";
    
    public static Error NotFoundWashingMachineWithCode => Error.NotFound(
        ComponentSlug + "NotFoundWashingMachineWithCode",
        "Myčka s tímto kódem neexistuje."
    );
    
    public static Error NotFoundWashingMachineWithLineQueueCode => Error.NotFound(
        ComponentSlug + "NotFoundWashingMachineWithLineQueueCode",
        "Myčka s tímto kódem fronty neexistuje."
    );
    
    public static Error ValidationKitWithCodeDoesNotExistInSettings => Error.Validation(
        ComponentSlug + "ValidationKitWithCodeDoesNotExistInSettings",
        "Kit s tímto kódem neexistuje."
    );
    
    public static Error ValidationPackagingWithCodeDoesNotExistInSettings => Error.Validation(
        ComponentSlug + "ValidationPackagingWithCodeDoesNotExistInSettings",
        "Obal s tímto kódem neexistuje."
    );
    
    public static Error ValidationPackagingIsNotListedInKitPackagings => Error.Validation(
        ComponentSlug + "ValidationPackagingIsNotListedInKitPackagings",
        "Obal není obsažen v kitu."
    );
    
    public static Error ValidationLineQueueDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationLineQueueDoesNotExist",
        "Myčka s tímto kódem fronty neexistuje."
    );
    
    public static Error ValidationBatchIsNotPresentInTheQueue => Error.Validation(
        ComponentSlug + "ValidationBatchIsNotPresentInTheQueue",
        "Dávka neexistuje v dané frontě myčky."
    );
    
    public static Error ValidationWashingMachineIsNotPresentedInFilteredWashingMachines => Error.Validation(
        ComponentSlug + "ValidationWashingMachineIsNotPresentedInFilteredWashingMachines",
        "Obal nelze prát na lajně, kterou jste vybrali."
    );
    
    public static Error ValidationInvalidSpeedLevel => Error.Validation(
        ComponentSlug + "ValidationInvalidSpeedLevel",
        $"Nestavená rychlost obalu nesouhlasí s rychlostí myčky."
    );
    
    public static Error ValidationKitCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationKitCodeIsRequired",
        $"Kód kitu je povinné pole."
    );
    
    public static Error ValidationPackagingCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationKitCodeIsRequired",
        $"Kód obalu je povinné pole."
    );
    
    public static Error ValidationBatchIdIsRequired => Error.Validation(
        ComponentSlug + "ValidationBatchIdIsRequired",
        $"Id dávky je povidnné pole."
    );
    
    public static Error ValidationKitsCountToChangeMustNotBeEqualTo0 => Error.Validation(
        ComponentSlug + "ValidationKitsCountToChangeMustNotBeEqualTo0",
        $"Musíte nastavit počet kitů, o které chcete změnit dávku."
    );
    
    public static Error ValidationKitsCountGreaterThan0 => Error.Validation(
        ComponentSlug + "ValidationKitsCountGreaterThan0",
        $"Počet kitů musí být větší než 0."
    );
    
    public static Error ValidationLineQueueCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineQueueCodeIsRequired",
        $"Kód lajny je povinné pole."
    );
    
    public static Error ValidationWashingMachineCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationWashingMachineCodeIsRequired",
        $"Kód myčky je povinné pole."
    );
    
    public static Error ValidationSisterPackagingCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationSisterPackagingCodeIsRequired",
        $"Kód obalu 2 je povinné pole."
    );
    
    public static Error ValidationWashingMachineAlreadyExists => Error.Validation(
        ComponentSlug + "ValidationWashingMachineAlreadyExists",
        $"Myčka s tímto kódem již existuje."
    );
    
    public static Error ValidationInvalidLineQueues => Error.Validation(
        ComponentSlug + "ValidationInvalidLineQueues",
        $"Lajny myčky nejsou platné."
    );
    
    public static Error ValidationLineCodeIsRequired => Error.Validation(
        ComponentSlug + "ValidationLineCodeIsRequired",
        $"Kód lajny je povinné pole."
    );
    
    public static Error ValidationSingleLineWashingMachine => Error.Validation(
        ComponentSlug + "ValidationSingleLineWashingMachine",
        $"Myčka má pouze jednu lajnu."
    );
    
    public static Error ValidationBatchIsNotSisterBatch => Error.Validation(
        ComponentSlug + "ValidationBatchIsNotSisterBatch",
        $"Dávka není sesterská. Nemůžeme vykonat akci pro dávku."
    );
    
    public static Error ValidationCannotMoveSingleBatch => Error.Validation(
        ComponentSlug + "ValidationCannotMoveSingleBatch",
        $"Právě praná dávka se nemůže posunout."
    );
    
    public static Error ValidationWashingMachineDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationWashingMachineDoesNotExist",
        "Myčka s tímto kódem neexistuje."
    );
}