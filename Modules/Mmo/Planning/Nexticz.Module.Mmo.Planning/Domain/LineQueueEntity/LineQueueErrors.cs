using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Planning.Domain.LineQueueEntity;

internal abstract class LineQueueErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-planning-domain-washingMachineLineQueueEntity-";
    
    public static readonly Error ValidationBatchStatusHasToBeInQueue = Error.Validation(
        ComponentSlug + "ValidationBatchStatusHasToBeInQueue",
        "Status dávky musí být 'In queue'.");
    
    public static readonly Error ValidationBatchNotFoundInTheList = Error.Validation(
        ComponentSlug + "ValidationBatchNotFoundInTheList",
        "Dávku nebyla nalezena ve frontě.");
    
    public static readonly Error ValidationBatchCannotBeMovedOutOfRange = Error.Validation(
        ComponentSlug + "ValidationBatchCannotBeMovedOutOfRange",
        "Dávku nemůžeme přesunout mimo frontu.");
    
    public static readonly Error ValidationBatchCannotBeMovedBeforeBatchWichIsWashing = Error.Validation(
        ComponentSlug + "ValidationBatchCannotBeMovedBeforeBatchWichIsWashing",
        "Nemůžete posunout dávku před dávku, která se právě myje.");
    
    public static readonly Error ValidationWashingBatchCannotBeRemoved = Error.Validation(
        ComponentSlug + "ValidationWashingBatchCannotBeRemoved",
        "Nemůžete smazat dávku, která se právě myje.");
    
    public static readonly Error ValidationBatchCannotBeFinishedIfItIsNotInWashing = Error.Validation(
        ComponentSlug + "ValidationBatchCannotBeFinished",
        "Nemůžete ukončit dávku, která se nemyje.");
    
    public static readonly Error ValidationCannotStartWashingBatchBecauseItIsNotNextInQueue = Error.Validation(
        ComponentSlug + "ValidationCannotStartWashingBatchBecauseItIsNotNextInQueue",
        $"Nemůžete začít prát obal, protože není další na řadě.");
}