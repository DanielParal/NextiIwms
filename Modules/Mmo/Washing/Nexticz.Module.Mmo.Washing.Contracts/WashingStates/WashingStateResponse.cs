using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Washing.Contracts.WashingStates;

public record WashingStateResponse(
    [property: Required] WashingStateShiftContract LastShift,
    [property: Required] WashingStateShiftContract NextToLastShift,
    [property: Required] WashingStateMachineContract[] WashingMachines);