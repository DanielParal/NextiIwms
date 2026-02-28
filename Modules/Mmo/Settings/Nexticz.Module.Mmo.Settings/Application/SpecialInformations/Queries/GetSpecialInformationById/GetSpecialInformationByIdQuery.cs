using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;

internal record GetSpecialInformationByIdQuery(Guid Id) : IRequest<ErrorOr<SpecialInformation>>;