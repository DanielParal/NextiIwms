namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.CreateUpdateBulkAddressLocations;

internal record CreateUpdateBulkAddressLocationsResponse(
    CreateUpdateBulkAddressLocationsResponseItem[] SucceededItems,
    CreateUpdateBulkAddressLocationsResponseItem[] FailedItems);

internal record CreateUpdateBulkAddressLocationsResponseItem(string AdmCode, Guid? Id, string? ErrorMessage);