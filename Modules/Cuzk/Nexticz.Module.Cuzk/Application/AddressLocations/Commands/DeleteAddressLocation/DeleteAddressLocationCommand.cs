using ErrorOr;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.DeleteAddressLocation;

internal record DeleteAddressLocationCommand(string AdmCode) : ICuzkCommand<ErrorOr<Success>>;