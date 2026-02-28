using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Commands.CreateAddressLocation;

internal class CreateAddressLocationCommandHandler(
    ILogger<CreateAddressLocationCommandHandler> logger,
    ISender sender,
    ICuzkUnitOfWork unitOfWork,
    IClock clock,
    IAddressLocationSearch addressLocationSearch) : IRequestHandler<CreateAddressLocationCommand, ErrorOr<AddressLocation>>
{
    public async Task<ErrorOr<AddressLocation>> Handle(CreateAddressLocationCommand request, CancellationToken cancellationToken)
    {
        var existing = await sender.Send(new GetAddressLocationByAdmCodeQuery(request.Request.AdmCode), cancellationToken);
        if (existing.HasValue())
        {
            logger.LogWarning("[Cuzk] [CreateAddressLocationCommandHandler] - AddressLocation cannot be created because ADM code already exists, Request: {@Request}",
                request.Request);
            return AddressLocationErrors.ValidationAddressLocationWithAdmCodeAlreadyExists;
        }

        var created = AddressLocation.CreateFrom(
            request.Request.AdmCode,
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

        if (created.IsError)
        {
            logger.LogWarning("[Cuzk] [CreateAddressLocationCommandHandler] - AddressLocation cannot be created because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, Request: {@Request}",
                created.FirstError.Code, created.FirstError.Description, request.Request);
            return created.Errors;
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

        logger.LogInformation("[Cuzk] [CreateAddressLocationCommandHandler] AddressLocation created, AddressLocationId: {AddressLocationId}, Request: {@Request}",
            created.Value.Id, request.Request);

        return created;
    }
}