using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftStatus;

internal record GetShiftStatusQuery(Shift Shift, UnapprovedShiftStatusView[] UnapprovedShiftStatusViews) : IRequest<ShiftStatus>;