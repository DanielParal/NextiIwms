using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsByAdmCodes;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.CreateUpdateBulkAddressLocations;

internal class CreateUpdateBulkAddressLocationsCommandHandler(
    ILogger<CreateUpdateBulkAddressLocationsCommandHandler> logger,
    ISender sender,
    ICuzkUnitOfWork unitOfWork,
    IClock clock,
    IAddressLocationSearch addressLocationSearch) : IRequestHandler<CreateUpdateBulkAddressLocationsCommand, CreateUpdateBulkAddressLocationsResponse>
{
    public async Task<CreateUpdateBulkAddressLocationsResponse> Handle(CreateUpdateBulkAddressLocationsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Cuzk] [Start] [CreateUpdateBulkAddressLocationsCommandHandler]");
        
        var succeededItems = new List<CreateUpdateBulkAddressLocationsResponseItem>();
        var failedItems = new List<CreateUpdateBulkAddressLocationsResponseItem>();
        
        if (request.BulkRequests.Length == 0)
        {
            logger.LogInformation("[Cuzk] [End] [CreateUpdateBulkAddressLocationsCommandHandler] - there are no requests to create/update");
            return new CreateUpdateBulkAddressLocationsResponse(
                succeededItems.ToArray(), 
                failedItems.ToArray());
        }
        
        var upperAdmCodes = request.BulkRequests.Select(x => x.AdmCode.ToUpperInvariant()).Distinct().ToArray();
        var addressLocationsToProcess = await sender.Send(new GetAddressLocationsByAdmCodesQuery(upperAdmCodes), cancellationToken);
        var addressLocationsByAdm = addressLocationsToProcess.ToDictionary(x => x.AdmCode, StringComparer.InvariantCultureIgnoreCase);
        
        foreach (var bulkRequest in request.BulkRequests)
        {
            var hasExisting = addressLocationsByAdm.TryGetValue(bulkRequest.AdmCode, out var existingAddressLocation);

            var (item, isSuccess) = hasExisting
                ? await UpdateAddressLocationAsync(bulkRequest, existingAddressLocation!, cancellationToken)
                : await CreateAddressLocationAsync(bulkRequest, cancellationToken);

            if (isSuccess)
            {
                succeededItems.Add(item);
            }
            else
            {
                failedItems.Add(item);
            }
        }
        
        logger.LogInformation("[Cuzk] [End] [CreateUpdateBulkAddressLocationsCommandHandler] - AddressLocation bulk import created/updated, SucceededItemsCount: {ImportedCount}, FailedItemsCount: {FailedCount}",
            succeededItems.Count, failedItems.Count);
        
        return new CreateUpdateBulkAddressLocationsResponse(
            succeededItems.ToArray(), 
            failedItems.ToArray());
    }

    private async Task<(CreateUpdateBulkAddressLocationsResponseItem Item, bool IsSuccess)> UpdateAddressLocationAsync(
        CreateUpdateBulkAddressLocationsRequest bulkRequest, AddressLocation addressLocation, CancellationToken cancellationToken)
    {
        var hasSameProperties = addressLocation.HasSamePropertiesAs(
            bulkRequest.AdmCode,
            bulkRequest.MunicipalityCode,
            bulkRequest.MunicipalityName,
            bulkRequest.MunicipalityDistrictCode,
            bulkRequest.MunicipalityDistrictName,
            bulkRequest.MomcCode,
            bulkRequest.MomcName,
            bulkRequest.PragueDistrictCode,
            bulkRequest.PragueDistrictName,
            bulkRequest.StreetCode,
            bulkRequest.StreetName,
            bulkRequest.DistrictCode,
            bulkRequest.DistrictName,
            bulkRequest.CountryCode,
            bulkRequest.CountryName,
            bulkRequest.SoType,
            bulkRequest.NumberDescriptive,
            bulkRequest.NumberReference,
            bulkRequest.NumberReferenceChar,
            bulkRequest.ZipCode,
            bulkRequest.KrovakX,
            bulkRequest.KrovakY,
            bulkRequest.Latitude,
            bulkRequest.Longitude,
            bulkRequest.Altitude,
            bulkRequest.ValidFrom);
        
        if (hasSameProperties)
            return (new CreateUpdateBulkAddressLocationsResponseItem(addressLocation.AdmCode, addressLocation.Id, null), true);
        
        var updateResult = addressLocation.Update(
            bulkRequest.AdmCode,
            bulkRequest.MunicipalityCode,
            bulkRequest.MunicipalityName,
            bulkRequest.MunicipalityDistrictCode,
            bulkRequest.MunicipalityDistrictName,
            bulkRequest.MomcCode,
            bulkRequest.MomcName,
            bulkRequest.PragueDistrictCode,
            bulkRequest.PragueDistrictName,
            bulkRequest.StreetCode,
            bulkRequest.StreetName,
            bulkRequest.DistrictCode,
            bulkRequest.DistrictName,
            bulkRequest.CountryCode,
            bulkRequest.CountryName,
            bulkRequest.SoType,
            bulkRequest.NumberDescriptive,
            bulkRequest.NumberReference,
            bulkRequest.NumberReferenceChar,
            bulkRequest.ZipCode,
            bulkRequest.KrovakX,
            bulkRequest.KrovakY,
            bulkRequest.Latitude,
            bulkRequest.Longitude,
            bulkRequest.Altitude,
            bulkRequest.ValidFrom,
            clock.UtcNowOffset);

        if (updateResult.IsError)
        {
            logger.LogWarning("[Cuzk] [CreateUpdateBulkAddressLocationsCommandHandler] - AddressLocation cannot be updated because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, CreateRequest: {@Request}",
                updateResult.FirstError.Code, updateResult.FirstError.Description, bulkRequest);
            return (new CreateUpdateBulkAddressLocationsResponseItem(addressLocation.AdmCode, addressLocation.Id, updateResult.FirstError.Description), false); 
        }

        await addressLocationSearch.IndexAsync(addressLocation, cancellationToken);
        
        var updatedEvent = new AddressLocationUpdatedEvent(
            addressLocation.Id,
            addressLocation.AdmCode,
            addressLocation.MunicipalityCode,
            addressLocation.MunicipalityName,
            addressLocation.MunicipalityDistrictCode,
            addressLocation.MunicipalityDistrictName,
            addressLocation.MomcCode,
            addressLocation.MomcName,
            addressLocation.PragueDistrictCode,
            addressLocation.PragueDistrictName,
            addressLocation.StreetCode,
            addressLocation.StreetName,
            addressLocation.DistrictCode,
            addressLocation.DistrictName,
            addressLocation.CountryCode,
            addressLocation.CountryName,
            addressLocation.SoType,
            addressLocation.NumberDescriptive,
            addressLocation.NumberReference,
            addressLocation.NumberReferenceChar,
            addressLocation.ZipCode,
            addressLocation.KrovakX,
            addressLocation.KrovakY,
            addressLocation.Latitude,
            addressLocation.Longitude,
            addressLocation.Altitude,
            addressLocation.Slug,
            addressLocation.ValidFrom,
            clock.UtcNowOffset);

        unitOfWork.AppendEvent(addressLocation.Id, updatedEvent);
        
        return (new CreateUpdateBulkAddressLocationsResponseItem(addressLocation.AdmCode, addressLocation.Id, null), true);    
    }
    
    private async Task<(CreateUpdateBulkAddressLocationsResponseItem Item, bool IsSuccess)> CreateAddressLocationAsync(CreateUpdateBulkAddressLocationsRequest bulkRequest, CancellationToken cancellationToken)
    {
        var created = AddressLocation.CreateFrom(
                bulkRequest.AdmCode,
                bulkRequest.MunicipalityCode,
                bulkRequest.MunicipalityName,
                bulkRequest.MunicipalityDistrictCode,
                bulkRequest.MunicipalityDistrictName,
                bulkRequest.MomcCode,
                bulkRequest.MomcName,
                bulkRequest.PragueDistrictCode,
                bulkRequest.PragueDistrictName,
                bulkRequest.StreetCode,
                bulkRequest.StreetName,
                bulkRequest.DistrictCode,
                bulkRequest.DistrictName,
                bulkRequest.CountryCode,
                bulkRequest.CountryName,
                bulkRequest.SoType,
                bulkRequest.NumberDescriptive,
                bulkRequest.NumberReference,
                bulkRequest.NumberReferenceChar,
                bulkRequest.ZipCode,
                bulkRequest.KrovakX,
                bulkRequest.KrovakY,
                bulkRequest.Latitude,
                bulkRequest.Longitude,
                bulkRequest.Altitude,
                bulkRequest.ValidFrom,
                clock.UtcNowOffset);

            if (created.IsError)
            {
                logger.LogWarning("[Cuzk] [CreateUpdateBulkAddressLocationsCommandHandler] AddressLocation cannot be created because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, CreateRequest: {@Request}",
                    created.FirstError.Code, created.FirstError.Description, bulkRequest);
                return (new CreateUpdateBulkAddressLocationsResponseItem(bulkRequest.AdmCode, null, created.FirstError.Description), false);
            }

            await addressLocationSearch.IndexAsync(created.Value, cancellationToken);

            var createdEvent = new AddressLocationCreatedEvent(
                created.Value.Id,
                created.Value.AdmCode,
                created.Value.MunicipalityCode,
                created.Value.MunicipalityName,
                created.Value.MunicipalityDistrictCode,
                created.Value.MunicipalityDistrictName,
                created.Value.MomcCode,
                created.Value.MomcName,
                created.Value.PragueDistrictCode,
                created.Value.PragueDistrictName,
                created.Value.StreetCode,
                created.Value.StreetName,
                created.Value.DistrictCode,
                created.Value.DistrictName,
                created.Value.CountryCode,
                created.Value.CountryName,
                created.Value.SoType,
                created.Value.NumberDescriptive,
                created.Value.NumberReference,
                created.Value.NumberReferenceChar,
                created.Value.ZipCode,
                created.Value.KrovakX,
                created.Value.KrovakY,
                created.Value.Latitude,
                created.Value.Longitude,
                created.Value.Altitude,
                created.Value.Slug,
                created.Value.ValidFrom,
                created.Value.CreatedAt);

            unitOfWork.StartStream<AddressLocationCreatedEvent, AddressLocation>(created.Value.Id, createdEvent);
            return (new CreateUpdateBulkAddressLocationsResponseItem(bulkRequest.AdmCode, created.Value.Id, null), true);       
    }
}