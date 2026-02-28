using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalitiesWhichShouldBeImported;

internal record GetMunicipalitiesWhichShouldBeImportedQuery() : IRequest<Municipality[]>;