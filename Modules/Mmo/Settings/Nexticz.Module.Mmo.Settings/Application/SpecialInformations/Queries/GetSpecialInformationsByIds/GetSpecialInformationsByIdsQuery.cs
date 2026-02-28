using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationsByIds;

internal record GetSpecialInformationsByIdsQuery(Guid[] Ids) : IRequest<SpecialInformation[]>;