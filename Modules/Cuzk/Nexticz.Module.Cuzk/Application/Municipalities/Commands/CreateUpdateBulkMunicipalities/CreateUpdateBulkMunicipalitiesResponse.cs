namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateUpdateBulkMunicipalities;

internal record CreateUpdateBulkMunicipalitiesResponse(
    CreateUpdateBulkMunicipalitiesResponseItem[] SucceededItems,
    CreateUpdateBulkMunicipalitiesResponseItem[] FailedItems);


internal record CreateUpdateBulkMunicipalitiesResponseItem(string Code, Guid? Id, string? ErrorMessage);