namespace Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;

public record BatchActivatedResponse(
    BatchContract Batch,
    string WashingMachineCode,
    string LineCode,
    DateTimeOffset DateActivated);