using ErrorOr;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateMunicipality;

internal record CreateMunicipalityCommand(CreateMunicipalityRequest Request) : ICuzkCommand<ErrorOr<Municipality>>;