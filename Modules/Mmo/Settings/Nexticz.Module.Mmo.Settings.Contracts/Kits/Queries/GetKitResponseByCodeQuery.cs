using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;

public record GetKitResponseByCodeQuery(string Code) : IRequest<ErrorOr<KitResponse>>;