using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Kits.Queries;

public record GetCurrentKitSpecialInformationResponseByKitCodeQuery(string KitCode) : IRequest<KitSpecialInformationResponse?>;