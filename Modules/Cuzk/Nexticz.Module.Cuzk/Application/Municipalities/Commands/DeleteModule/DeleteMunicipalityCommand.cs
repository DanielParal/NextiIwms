using ErrorOr;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.DeleteModule;

internal record DeleteMunicipalityCommand(string Code) : ICuzkCommand<ErrorOr<Success>>;