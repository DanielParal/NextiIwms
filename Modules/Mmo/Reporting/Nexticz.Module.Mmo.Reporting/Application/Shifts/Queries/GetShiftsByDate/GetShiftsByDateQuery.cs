using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftsByDate;

internal record GetShiftsByDateQuery(DateOnly SelectedDate) : IRequest<Shift[]>;