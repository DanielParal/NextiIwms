using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypes;

internal record GetInactivityTypesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<InactivityType>>;