using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalitiesByCodes;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateUpdateBulkMunicipalities;

internal class CreateUpdateBulkMunicipalitiesCommandHandler(
    ILogger<CreateUpdateBulkMunicipalitiesCommandHandler> logger,
    ISender sender,
    ICuzkUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateUpdateBulkMunicipalitiesCommand, CreateUpdateBulkMunicipalitiesResponse>
{
    public async Task<CreateUpdateBulkMunicipalitiesResponse> Handle(CreateUpdateBulkMunicipalitiesCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Cuzk] [Start] [CreateUpdateBulkMunicipalitiesCommandHandler]");
        
        var succeededItems = new List<CreateUpdateBulkMunicipalitiesResponseItem>();
        var failedItems = new List<CreateUpdateBulkMunicipalitiesResponseItem>();
        
        if (request.BulkRequests.Length == 0)
        {
            logger.LogInformation("[Cuzk] [End] [CreateUpdateBulkMunicipalitiesCommandHandler] - there are no requests to create/update");
            return new CreateUpdateBulkMunicipalitiesResponse(
                succeededItems.ToArray(), 
                failedItems.ToArray());
        }  
        
        var upperCodes = request.BulkRequests.Select(x => x.MunicipalityCode.ToUpperInvariant()).Distinct().ToArray();
        var municipalitiesToProcess = await sender.Send(new GetMunicipalitiesByCodesQuery(upperCodes), cancellationToken);
        var municipalitiesByCode = municipalitiesToProcess.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);
        
        foreach (var bulkRequest in request.BulkRequests)
        {
            var hasExisting = municipalitiesByCode.TryGetValue(bulkRequest.MunicipalityCode, out var existingMunicipality);

            var (item, isSuccess) = hasExisting
                ? UpdateMunicipality(bulkRequest, existingMunicipality!)
                : CreateMunicipality(bulkRequest);

            if (isSuccess)
            {
                succeededItems.Add(item);
            }
            else
            {
                failedItems.Add(item);
            }
        }
        
        logger.LogInformation("[Cuzk] [End] [CreateUpdateBulkMunicipalitiesCommandHandler] - Municipality bulk import created/updated, SucceededItemsCount: {ImportedCount}, FailedItemsCount: {FailedCount}",
            succeededItems.Count, failedItems.Count);
        
        return new CreateUpdateBulkMunicipalitiesResponse(
            succeededItems.ToArray(), 
            failedItems.ToArray());
    }

    private (CreateUpdateBulkMunicipalitiesResponseItem Item, bool IsSuccess) UpdateMunicipality(
        CreateUpdateBulkMunicipalitiesRequest bulkRequest, Municipality municipality)
    {
        var hasSameProperties = municipality.HasSamePropertiesAs(
            bulkRequest.MunicipalityCode,
            bulkRequest.MunicipalityName,
            bulkRequest.MunicipalityStatus,
            bulkRequest.PouCode,
            bulkRequest.PouName,
            bulkRequest.OrpCode,
            bulkRequest.OrpName,
            bulkRequest.DistrictCode,
            bulkRequest.DistrictName,
            bulkRequest.VuscCode,
            bulkRequest.VuscName);
        
        if (hasSameProperties)
            return (new CreateUpdateBulkMunicipalitiesResponseItem(municipality.Code, municipality.Id, null), true);

        var updateResult = municipality.Update(
            bulkRequest.MunicipalityCode,
            bulkRequest.MunicipalityName,
            bulkRequest.MunicipalityStatus,
            bulkRequest.PouCode,
            bulkRequest.PouName,
            bulkRequest.OrpCode,
            bulkRequest.OrpName,
            bulkRequest.DistrictCode,
            bulkRequest.DistrictName,
            bulkRequest.VuscCode,
            bulkRequest.VuscName,
            municipality.ShouldImportAddressLocation
        );
        
        if (updateResult.IsError)
        {
            logger.LogWarning("[Cuzk] [CreateUpdateBulkMunicipalitiesCommandHandler] - Municipality cannot be updated because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, CreateRequest: {@Request}",
                updateResult.FirstError.Code, updateResult.FirstError.Description, bulkRequest);
            return (new CreateUpdateBulkMunicipalitiesResponseItem(municipality.Code, municipality.Id, updateResult.FirstError.Description), false); 
        }
        
        var updatedEvent = new MunicipalityUpdatedEvent(
            municipality.Id,
            municipality.Code,
            municipality.Name,
            municipality.Status,
            municipality.PouCode,
            municipality.PouName,
            municipality.OrpCode,
            municipality.OrpName,
            municipality.DistrictCode,
            municipality.DistrictName,
            municipality.VuscCode,
            municipality.VuscName,
            municipality.ShouldImportAddressLocation,
            clock.UtcNowOffset);

        unitOfWork.AppendEvent(municipality.Id, updatedEvent);
        
        return (new CreateUpdateBulkMunicipalitiesResponseItem(municipality.Code, municipality.Id, null), true);  
    }

    private (CreateUpdateBulkMunicipalitiesResponseItem Item, bool IsSuccess) CreateMunicipality(CreateUpdateBulkMunicipalitiesRequest bulkRequest)
    {
        var created = Municipality.CreateFrom(
            bulkRequest.MunicipalityCode,
            bulkRequest.MunicipalityName,
            bulkRequest.MunicipalityStatus,
            bulkRequest.PouCode,
            bulkRequest.PouName,
            bulkRequest.OrpCode,
            bulkRequest.OrpName,
            bulkRequest.DistrictCode,
            bulkRequest.DistrictName,
            bulkRequest.VuscCode,
            bulkRequest.VuscName,
            bulkRequest.ShouldImportAddressLocation,
            clock.UtcNowOffset);
        
        if (created.IsError)
        {
            logger.LogWarning("[Cuzk] [CreateUpdateBulkMunicipalitiesCommandHandler] - Municipality cannot be created because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, CreateRequest: {@Request}",
                created.FirstError.Code, created.FirstError.Description, bulkRequest);
            return (new CreateUpdateBulkMunicipalitiesResponseItem(bulkRequest.MunicipalityCode, null, created.FirstError.Description), false);
        }
        
        var municipalityCreatedEvent = new MunicipalityCreatedEvent(
            created.Value.Id, created.Value.Code, created.Value.Name,
            created.Value.Status, created.Value.PouCode, created.Value.PouName,
            created.Value.OrpCode, created.Value.OrpName, created.Value.DistrictCode,
            created.Value.DistrictName, created.Value.VuscCode, created.Value.VuscName,
            created.Value.ShouldImportAddressLocation, created.Value.CreatedAt);
        
        unitOfWork.StartStream<MunicipalityCreatedEvent, Municipality>(created.Value.Id, municipalityCreatedEvent);
        return (new CreateUpdateBulkMunicipalitiesResponseItem(bulkRequest.MunicipalityCode, created.Value.Id, null), true);  
    }
}