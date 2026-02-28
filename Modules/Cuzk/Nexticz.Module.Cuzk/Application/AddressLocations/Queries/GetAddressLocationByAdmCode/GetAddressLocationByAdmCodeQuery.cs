using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;

internal record GetAddressLocationByAdmCodeQuery(string AdmCode) : IRequest<ErrorOr<AddressLocation>>;