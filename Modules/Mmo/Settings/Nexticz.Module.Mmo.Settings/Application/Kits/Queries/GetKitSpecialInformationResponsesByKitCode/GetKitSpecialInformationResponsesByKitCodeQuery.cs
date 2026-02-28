using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitSpecialInformationResponsesByKitCode;

internal record GetKitSpecialInformationResponsesByKitCodeQuery(string KitCode) : IRequest<KitSpecialInformationResponse[]>;