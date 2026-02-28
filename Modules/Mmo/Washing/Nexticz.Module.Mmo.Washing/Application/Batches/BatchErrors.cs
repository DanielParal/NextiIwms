using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Washing.Application.Batches;

internal abstract class BatchErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-washing-api-batchesService-";
    
    public static Error BatchNotFound => Error.NotFound(
        ComponentSlug + "BatchNotFound",
        "Dávka nebyla nalezena."
    );
    
    public static Error ValidationIdIsRequired => Error.Validation(
        ComponentSlug + "ValidationIdIsRequired",
        "Id je povinné pole."
    );
    
    public static Error ValidationBatchDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationBatchDoesNotExist",
        "Dávka s daným kódem neexistuje."
    );
    
    public static Error ValidationWashingMachineDoesNotExist => Error.Validation(
        ComponentSlug + "ValidationWashingMachineDoesNotExist",
        "Myčka s tímto kódem neexistuje."
    );
    
    public static Error ValidationCannotFinishKitInShortTimePeriod => Error.Validation(
        ComponentSlug + "ValidationCannotFinishKitInShortTimePeriod",
        "Nemůžeme ukončit kit. Kit se může ukončit 2 minuty po předchozím kitu."
    );
    
    public static Error ValidationBatchDoesNotHaveSpecialInformation => Error.Validation(
        ComponentSlug + "ValidationBatchDoesNotHaveSpecialInformation",
        "Dávka neobsahuje žádné speciální informace."
    );
    
    public static Error NotFoundKitInstructionFile => Error.NotFound(
        ComponentSlug + "ValidationKitInstructionFileNotFound",
        "Balící předpis pro tento kit nebyl nalezen."
    );
    
    public static Error NotFoundSpecialInformationFile => Error.NotFound(
        ComponentSlug + "NotFoundSpecialInformationFile",
        "Soubor pro speciální informaci nebyl nalezen."
    );

    public static Error ValidationBatchIsNotConfirmedByWorker => Error.Validation(
        ComponentSlug + "ValidationBatchIsNotConfirmedByWorker",
        "Dávka není potvrzena pracovníkem."
    );
    
    public static Error ValidationThereIsNoWorkerAtTheLine => Error.Validation(
        ComponentSlug + "ValidationThereIsNoWorkerAtTheLine",
        "Žádný pracovník není na lajně."
    );
}