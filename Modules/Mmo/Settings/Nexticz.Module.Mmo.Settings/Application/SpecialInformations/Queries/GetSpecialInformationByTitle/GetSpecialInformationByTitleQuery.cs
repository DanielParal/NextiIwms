using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationByTitle;

internal record GetSpecialInformationByTitleQuery(string Title) : IRequest<ErrorOr<SpecialInformation>>;