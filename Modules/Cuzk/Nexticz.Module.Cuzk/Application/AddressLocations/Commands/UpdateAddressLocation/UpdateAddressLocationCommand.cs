using ErrorOr;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.UpdateAddressLocation;

internal record UpdateAddressLocationCommand(string AdmCode, UpdateAddressLocationRequest Request) : ICuzkCommand<ErrorOr<Success>>;