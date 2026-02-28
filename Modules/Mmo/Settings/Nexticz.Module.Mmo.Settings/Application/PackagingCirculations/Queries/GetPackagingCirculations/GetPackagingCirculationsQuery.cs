using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculations;

internal record GetPackagingCirculationsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<PackagingCirculation>>;