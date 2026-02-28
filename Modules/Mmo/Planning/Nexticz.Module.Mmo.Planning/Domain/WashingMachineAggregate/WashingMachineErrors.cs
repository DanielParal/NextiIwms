using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

internal abstract class WashingMachineErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-planning-domain-washingMachineAggregate-";
    
    public static Error ValidationWashingMachineLineNotFound(string code) => Error.Validation(
        ComponentSlug + "ValidationWashingMachineLineNotFound",
        $"Lajna s kódem {code} neexistuje.");
    
    public static Error ValidationCannotScheduleSimultaneousBatchOnOneLineWashingMachine = Error.Validation(
        ComponentSlug + "ValidationCannotScheduleSimultaneousBatchOnOneLineWashingMachine",
        $"Nemůžete mýt dávku s dvěma obaly na jednostopé myčce.");
    
    public static Error ValidationSisterBatchesIdsMustMatch = Error.Validation(
        ComponentSlug + "ValidationSisterBatchesIdsMustMatch",
        $"Neshoduje se id dávky s obalem 1 a sesterské id dávky s obalem 2.");
    
    public static Error ValidationBatchWithSisterIdMustBeScheduledTogether = Error.Validation(
        ComponentSlug + "ValidationBatchWithSisterIdMustBeScheduledTogether",
        $"Kit s dvěma obaly k mytí musí být naplánovány společně.");
    
    public static Error ValidationBatchIdNotFoundInQueue = Error.Validation(
        ComponentSlug + "ValidationBatchIdNotFoundInQueue",
        $"Dávka nenalezena v žádné frontě.");
    
    public static Error ValidationSisterBatchIdNotFoundInQueue = Error.Validation(
        ComponentSlug + "ValidationSisterBatchIdNotFoundInQueue",
        $"Sesterská dávka nenalezena v žádné frontě.");
    
    public static Error ValidationBatchIsNotSisterCannotDetach = Error.Validation(
        ComponentSlug + "ValidationBatchIsNotSisterCannotDetach",
        $"Dávka není sesterská. Nemůžeme ji rozdělit.");
    
    public static Error ValidationWashingMachineHasOnlyOneLine = Error.Validation(
        ComponentSlug + "ValidationWashingMachineHasOnlyOneLine",
        $"Myčka má pouze jednu lajnu.");
}