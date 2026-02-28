using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;


namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformations;

internal record GetSpecialInformationsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<SpecialInformation>>;