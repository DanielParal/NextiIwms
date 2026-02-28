using MediatR;
using Nexticz.Module.Mmo.Reporting.Contracts.Shifts;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSelectionsByDate;

internal record GetShiftSelectionsByDateQuery(DateOnly SelectedDate) : IRequest<ShiftSelectionResponse[]>;