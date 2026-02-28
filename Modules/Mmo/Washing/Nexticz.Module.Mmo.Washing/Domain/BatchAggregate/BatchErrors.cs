using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

internal abstract class BatchErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-washing-domain-batchesService-";
    
    public static Error ValidationKitDoesNotExistInBatch = Error.Validation(
        ComponentSlug + "ValidationKitDoesNotExistInBatch",
        "Kit neexistuje v dané dávce.");
    
    public static Error ValidationKitIsNotInWashing = Error.Validation(
        ComponentSlug + "ValidationKitIsNotInWashing",
        "Kit není ve statusu mytí. Nemůžeme ho dokončit.");
    
    public static Error ValidationBatchIsNotConfirmedByWorker => Error.Validation(
        ComponentSlug + "ValidationBatchIsNotConfirmedByWorker",
        "Dávka není potvrzena pracovníkem."
    );
}