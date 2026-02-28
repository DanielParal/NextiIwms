using MediatR;
using Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftSummaryById;

internal record GetShiftSummaryByIdQuery(Shift Shift) : IRequest<ShiftSummary>;