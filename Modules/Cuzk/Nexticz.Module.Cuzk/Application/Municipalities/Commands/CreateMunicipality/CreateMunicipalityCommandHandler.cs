using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.CreateMunicipality;

internal class CreateMunicipalityCommandHandler(
    ILogger<CreateMunicipalityCommandHandler> logger,
    ISender sender,
    ICuzkUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateMunicipalityCommand, ErrorOr<Municipality>>
{
    public async Task<ErrorOr<Municipality>> Handle(CreateMunicipalityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Request.MunicipalityCode))
        {
            logger.LogWarning("Cuzk - Municipality cannot be created because code is required, Request: {@Request}", 
                request.Request);
            return MunicipalityErrors.ValidationMunicipalityCodeIsRequired;
        }
        
        var existingMunicipality = await sender.Send(new GetMunicipalityByCodeQuery(request.Request.MunicipalityCode), cancellationToken);
        if (existingMunicipality.HasValue())
        {
            logger.LogWarning("Cuzk - Municipality cannot be created because code already exists, Request: {@Request}", 
                request.Request);
            return MunicipalityErrors.ValidationMunicipalityWithCodeAlreadyExists;
        }
        
        var createdMunicipality = Municipality.CreateFrom(
            request.Request.MunicipalityCode,
            request.Request.MunicipalityName,
            request.Request.MunicipalityStatus,
            request.Request.PouCode,
            request.Request.PouName,
            request.Request.OrpCode,
            request.Request.OrpName,
            request.Request.DistrictCode,
            request.Request.DistrictName,
            request.Request.VuscCode,
            request.Request.VuscName,
            request.Request.ShouldImportAddressLocation,
            clock.UtcNowOffset);

        if (createdMunicipality.IsError)
        {
            logger.LogWarning("Cuzk - Municipality cannot be created because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, Request: {@Request}", 
                createdMunicipality.FirstError.Code, createdMunicipality.FirstError.Description, request.Request);
            return createdMunicipality.Errors;
        }
        
        var municipalityCreatedEvent = new MunicipalityCreatedEvent(
            createdMunicipality.Value.Id, createdMunicipality.Value.Code, createdMunicipality.Value.Name,
            createdMunicipality.Value.Status, createdMunicipality.Value.PouCode, createdMunicipality.Value.PouName,
            createdMunicipality.Value.OrpCode, createdMunicipality.Value.OrpName, createdMunicipality.Value.DistrictCode,
            createdMunicipality.Value.DistrictName, createdMunicipality.Value.VuscCode, createdMunicipality.Value.VuscName,
            createdMunicipality.Value.ShouldImportAddressLocation, createdMunicipality.Value.CreatedAt);
        
        unitOfWork.StartStream<MunicipalityCreatedEvent, Municipality>(createdMunicipality.Value.Id, municipalityCreatedEvent);
        
        logger.LogInformation("Cuzk - Municipality created, MunicipalityId: {MunicipalityId}, Request: {@Request}", 
            createdMunicipality.Value.Id, request.Request);
        
        return createdMunicipality;       
    }
}