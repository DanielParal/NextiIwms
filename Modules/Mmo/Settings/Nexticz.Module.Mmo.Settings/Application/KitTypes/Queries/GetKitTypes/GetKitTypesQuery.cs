using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypes;

internal record GetKitTypesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<KitType>>;