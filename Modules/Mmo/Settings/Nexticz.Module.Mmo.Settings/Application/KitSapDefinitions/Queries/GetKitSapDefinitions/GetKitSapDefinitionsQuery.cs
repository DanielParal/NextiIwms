using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;


namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitions;

internal record GetKitSapDefinitionsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<KitSapDefinition>>;