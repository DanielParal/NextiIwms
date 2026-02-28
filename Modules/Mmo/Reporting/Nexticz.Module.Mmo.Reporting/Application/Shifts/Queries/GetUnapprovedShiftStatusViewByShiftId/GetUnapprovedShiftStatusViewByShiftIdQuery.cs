using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViewByShiftId;

internal record GetUnapprovedShiftStatusViewByShiftIdQuery(Guid ShiftId) : IRequest<UnapprovedShiftStatusView?>;