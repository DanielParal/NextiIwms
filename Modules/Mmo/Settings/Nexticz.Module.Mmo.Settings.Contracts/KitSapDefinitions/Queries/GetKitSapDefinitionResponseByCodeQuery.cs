using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions.Queries;

public record GetKitSapDefinitionResponseByCodeQuery(string Code) : IRequest<ErrorOr<KitSapDefinitionResponse>>;