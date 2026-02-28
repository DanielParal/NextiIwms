using ErrorOr;
using MediatR;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsByAdmCodes;

internal record GetAddressLocationsByAdmCodesQuery(string[] AdmCodes) : IRequest<AddressLocation[]>;