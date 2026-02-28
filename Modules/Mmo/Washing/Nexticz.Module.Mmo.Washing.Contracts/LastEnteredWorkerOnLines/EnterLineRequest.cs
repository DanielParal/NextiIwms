using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.LastEnteredWorkerOnLines;

public record EnterLineRequest(
    [property: Required] int WorkerPin);