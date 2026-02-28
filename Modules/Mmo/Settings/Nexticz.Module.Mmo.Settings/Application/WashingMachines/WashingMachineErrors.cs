using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines;

internal abstract class WashingMachineErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-settings-api-washingMachineService-";
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Myčka s tímto kódem neexistuje");
    
    public static Error ValidationCodeIsEmpty => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Myčka s tímto kódem neexistuje");
    
    public static Error ValidationLengthGraterThanZero => Error.Validation(
        ComponentSlug + "validationLengthGraterThanZero",
        "Délka myčky musí být větší než 0");
    
    public static Error ValidationMinWidthGraterThanZero => Error.Validation(
        ComponentSlug + "validationMinWidthGraterThanZero",
        "Minimální šířka myčky musí být větší než 0");
    
    public static Error ValidationMaxWidthGraterThanZero => Error.Validation(
        ComponentSlug + "validationMaxWidthGraterThanZero",
        "Maximální šířka myčky musí být větší než 0 a musí být větší než minimální šířka myčky");
    
    public static Error ValidationMaxHeightGraterThanZero => Error.Validation(
        ComponentSlug + "validationMaxHeightGraterThanZero",
        "Maximální výška myčky musí být větší než 0");
    
    public static Error ValidationMaxWaterTemperatureGraterThanZero => Error.Validation(
        ComponentSlug + "validationMaxWaterTemperatureGraterThanZero",
        "Maximální teplota vody myčky musí být větší než 0");
    
    public static Error ValidationMaxAirTemperatureGraterThanZero => Error.Validation(
        ComponentSlug + "validationMaxAirTemperatureGraterThanZero",
        "Maximální teplota vzduchu myčky musí být větší než 0");
    
    public static Error ValidationNumberOfLinesEither1Or2 => Error.Validation(
        ComponentSlug + "validationNumberOfLinesEither1Or2",
        "Myčka může mít pouze 1 nebo 2 lajny");
    
    public static Error ValidationSpeed1GraterThanZero => Error.Validation(
        ComponentSlug + "validationSpeed1GraterThanZero",
        "Rychlost 1 musí být větší než 0");
    
    public static Error ValidationSpeed2GraterThanZero => Error.Validation(
        ComponentSlug + "validationSpeed2GraterThanZero",
        "Rychlost 2 musí být větší než 0");
    
    public static Error ValidationSpeed3GraterThanZero => Error.Validation(
        ComponentSlug + "validationSpeed3GraterThanZero",
        "Rychlost 3 musí být větší než 0");
    
    public static Error ValidationCodeAlreadyExists(string code) => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        $"Myčka s kódem '{code}' již existuje");
    
    public static Error ValidationNumberOfLinesDoNotMatch(int numberOfLines) => Error.Validation(
        ComponentSlug + "validationNumberOfLinesDoNotMatch",
        $"Myčka musí obsahovat počet lajn: {numberOfLines}.");
    
    public static Error ValidationAllLinesAreInactiveAndStatusWorking => Error.Validation(
        ComponentSlug + "validationAllLinesAreInactiveAndStatusWorking",
        "Všechny lajny jsou neaktivní a status myčky je 'Working'. Musíte změnit i status myčky.");
    
    public static Error ValidationLineCodesMismatch(string existingLineCodes, string requestedLineCodes) => Error.Validation(
        ComponentSlug + "validationLineCodesMismatch",
        $"Kódy lajn myček se neshodují s existujícími. Existující kódy: {existingLineCodes}, poslané kódy: {requestedLineCodes}.");
}