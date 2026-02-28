using MediatR;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.DevExtreme;


namespace Nexticz.Module.Portal.Application.Modules.Queries.GetModules;

internal record GetModulesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Domain.ModuleAggregate.Module>>;