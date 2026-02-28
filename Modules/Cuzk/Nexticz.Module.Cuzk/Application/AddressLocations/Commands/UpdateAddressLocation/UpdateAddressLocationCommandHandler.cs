using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.UpdateAddressLocation;

internal class UpdateAddressLocationCommandHandler(
    ISender sender,
    ILogger<UpdateAddressLocationCommandHandler> logger,
    ICuzkUnitOfWork unitOfWork,
    IClock clock,
    IAddressLocationSearch addressLocationSearch) : IRequestHandler<UpdateAddressLocationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateAddressLocationCommand request, CancellationToken cancellationToken)
    {
        var existing = await sender.Send(new GetAddressLocationByAdmCodeQuery(request.AdmCode), cancellationToken);
        if (existing.IsError)
        {
            logger.LogInformation("[Cuzk] [UpdateAddressLocationCommandHandler] Did not find object {ObjectName}. Nothing to update. Request: {@Request}",
                nameof(AddressLocation), request);
            return AddressLocationErrors.ValidationAddressLocationWithAdmCodeDoesNotExist;
        }

        var updateResult = existing.Value.Update(
            request.AdmCode,
            request.Request.MunicipalityCode,
            request.Request.MunicipalityName,
            request.Request.MunicipalityDistrictCode,
            request.Request.MunicipalityDistrictName,
            request.Request.MomcCode,
            request.Request.MomcName,
            request.Request.PragueDistrictCode,
            request.Request.PragueDistrictName,
            request.Request.StreetCode,
            request.Request.StreetName,
            request.Request.DistrictCode,
            request.Request.DistrictName,
            request.Request.CountryCode,
            request.Request.CountryName,
            request.Request.SoType,
            request.Request.NumberDescriptive,
            request.Request.NumberReference,
            request.Request.NumberReferenceChar,
            request.Request.ZipCode,
            request.Request.KrovakX,
            request.Request.KrovakY,
            request.Request.Latitude,
            request.Request.Longitude,
            request.Request.Altitude,
            request.Request.ValidFrom,
            clock.UtcNowOffset);

        if (updateResult.IsError)
        {
            logger.LogWarning("[Cuzk] [UpdateAddressLocationCommandHandler] Object {ObjectName} cannot be updated because of error, AddressLocationId: {AddressLocationId}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, Request: {@Request}",
                nameof(AddressLocation), existing.Value.Id, updateResult.FirstError.Code, updateResult.FirstError.Description, request);
            return updateResult.Errors;
        }
        
        await addressLocationSearch.IndexAsync(existing.Value, cancellationToken);

        var updatedEvent = new AddressLocationUpdatedEvent(
            existing.Value.Id,
            existing.Value.AdmCode,
            existing.Value.MunicipalityCode,
            existing.Value.MunicipalityName,
            existing.Value.MunicipalityDistrictCode,
            existing.Value.MunicipalityDistrictName,
            existing.Value.MomcCode,
            existing.Value.MomcName,
            existing.Value.PragueDistrictCode,
            existing.Value.PragueDistrictName,
            existing.Value.StreetCode,
            existing.Value.StreetName,
            existing.Value.DistrictCode,
            existing.Value.DistrictName,
            existing.Value.CountryCode,
            existing.Value.CountryName,
            existing.Value.SoType,
            existing.Value.NumberDescriptive,
            existing.Value.NumberReference,
            existing.Value.NumberReferenceChar,
            existing.Value.ZipCode,
            existing.Value.KrovakX,
            existing.Value.KrovakY,
            existing.Value.Latitude,
            existing.Value.Longitude,
            existing.Value.Altitude,
            existing.Value.Slug,
            existing.Value.ValidFrom,
            clock.UtcNowOffset);

        unitOfWork.AppendEvent(existing.Value.Id, updatedEvent);

        logger.LogInformation("[Cuzk] [UpdateAddressLocationCommandHandler] Object {ObjectName} with id: {Id} updated. Request: {@Request}",
            nameof(AddressLocation), existing.Value.Id, request);

        return Result.Success;
    }
}