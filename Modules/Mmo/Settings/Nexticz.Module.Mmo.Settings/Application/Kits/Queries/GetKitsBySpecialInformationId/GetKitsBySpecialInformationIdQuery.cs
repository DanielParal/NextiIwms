using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitsBySpecialInformationId;

internal record GetKitsBySpecialInformationIdQuery(Guid SpecialInformationId) : IRequest<Kit[]>;