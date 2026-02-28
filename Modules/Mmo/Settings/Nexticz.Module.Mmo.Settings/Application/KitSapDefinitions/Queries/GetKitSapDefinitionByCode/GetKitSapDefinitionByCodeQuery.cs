using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;

namespace Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;

internal record GetKitSapDefinitionByCodeQuery(string Code) : IRequest<ErrorOr<KitSapDefinition>>;