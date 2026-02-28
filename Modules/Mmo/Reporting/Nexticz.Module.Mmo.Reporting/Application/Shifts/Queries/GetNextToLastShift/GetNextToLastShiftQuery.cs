using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetNextToLastShift;

internal record GetNextToLastShiftQuery() : IRequest<ErrorOr<Shift>>;