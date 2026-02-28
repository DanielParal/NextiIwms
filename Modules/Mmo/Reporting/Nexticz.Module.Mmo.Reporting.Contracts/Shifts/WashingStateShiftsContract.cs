namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

public record WashingStateShiftsContract(
    WashingStateShiftSummaryContract? LastShift,
    WashingStateShiftSummaryContract? NextToLastShift);