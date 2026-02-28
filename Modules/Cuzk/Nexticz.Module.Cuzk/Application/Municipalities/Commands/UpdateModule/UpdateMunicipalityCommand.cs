using ErrorOr;
using Nexticz.Module.Cuzk.Contracts.Municipalities;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.UpdateModule;

internal record UpdateMunicipalityCommand(string Code, UpdateMunicipalityRequest Request) : ICuzkCommand<ErrorOr<Success>>;