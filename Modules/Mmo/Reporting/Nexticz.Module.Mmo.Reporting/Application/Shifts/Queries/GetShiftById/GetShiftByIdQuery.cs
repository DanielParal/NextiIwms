using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftById;

internal record GetShiftByIdQuery(Guid Id) : IRequest<ErrorOr<Shift>>;