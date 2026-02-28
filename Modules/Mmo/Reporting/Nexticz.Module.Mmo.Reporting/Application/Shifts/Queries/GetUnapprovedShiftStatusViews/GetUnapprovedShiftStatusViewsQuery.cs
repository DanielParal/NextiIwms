using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetUnapprovedShiftStatusViews;

internal record GetUnapprovedShiftStatusViewsQuery() : IRequest<UnapprovedShiftStatusView[]>;