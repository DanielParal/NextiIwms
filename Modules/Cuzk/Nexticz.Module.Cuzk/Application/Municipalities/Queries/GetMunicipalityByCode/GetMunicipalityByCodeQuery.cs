using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;

internal record GetMunicipalityByCodeQuery(string Code) : IRequest<ErrorOr<Municipality>>;