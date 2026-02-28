using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypes;

internal record GetPackagingTypesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<PackagingType>>;