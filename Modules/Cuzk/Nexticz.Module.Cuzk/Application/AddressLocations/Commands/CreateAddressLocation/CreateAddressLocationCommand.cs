using ErrorOr;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.CreateAddressLocation;

internal record CreateAddressLocationCommand(CreateAddressLocationRequest Request) : ICuzkCommand<ErrorOr<AddressLocation>>;