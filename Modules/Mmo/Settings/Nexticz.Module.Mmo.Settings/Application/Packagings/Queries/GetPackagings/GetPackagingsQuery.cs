using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagings;

internal record GetPackagingsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Packaging>>;