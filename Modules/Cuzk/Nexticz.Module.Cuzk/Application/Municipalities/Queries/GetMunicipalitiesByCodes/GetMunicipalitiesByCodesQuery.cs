using MediatR;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalitiesByCodes;

internal record GetMunicipalitiesByCodesQuery(string[] Codes) : IRequest<Municipality[]>;