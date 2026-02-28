using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Queries.GetShiftsByIds;

internal record GetShiftsByIdsQuery(Guid[] Ids) : IRequest<Shift[]>;