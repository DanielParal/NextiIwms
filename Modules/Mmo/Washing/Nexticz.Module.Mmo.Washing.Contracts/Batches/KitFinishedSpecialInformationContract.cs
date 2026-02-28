namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record KitFinishedSpecialInformationContract(
    Guid Id, string Title, string Description, bool HasFile, string WorkerName, DateTimeOffset DateConfirmed);