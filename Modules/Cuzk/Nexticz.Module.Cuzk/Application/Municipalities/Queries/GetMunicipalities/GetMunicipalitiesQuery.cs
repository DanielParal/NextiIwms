using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalities;

internal record GetMunicipalitiesQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Municipality>>;