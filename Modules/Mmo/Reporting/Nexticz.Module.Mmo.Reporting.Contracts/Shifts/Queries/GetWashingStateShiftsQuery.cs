using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Contracts.Shifts.Queries;

public record GetWashingStateShiftsQuery() : IRequest<WashingStateShiftsContract>;
