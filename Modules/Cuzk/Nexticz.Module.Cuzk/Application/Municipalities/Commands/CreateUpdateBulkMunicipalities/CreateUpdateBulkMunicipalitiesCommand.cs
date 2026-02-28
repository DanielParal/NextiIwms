using ErrorOr;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateUpdateBulkMunicipalities;

internal record CreateUpdateBulkMunicipalitiesCommand(CreateUpdateBulkMunicipalitiesRequest[] BulkRequests) : ICuzkCommand<CreateUpdateBulkMunicipalitiesResponse>;