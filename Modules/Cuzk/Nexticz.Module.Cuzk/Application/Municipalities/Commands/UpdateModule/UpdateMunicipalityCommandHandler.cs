using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.UpdateModule;

internal class UpdateMunicipalityCommandHandler(
    ISender sender,
    ILogger<UpdateMunicipalityCommandHandler> logger,
    ICuzkUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<UpdateMunicipalityCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateMunicipalityCommand request, CancellationToken cancellationToken)
    {
        var existingMunicipality = await sender.Send(new GetMunicipalityByCodeQuery(request.Code), cancellationToken);
        if (existingMunicipality.IsError)
        {
            logger.LogInformation("Cuzk - Did not find object {ObjectName}. Nothing to update. Request: {@Request}", 
                nameof(Municipality), request);
            return MunicipalityErrors.ValidationMunicipalityWithCodeDoesNotExist;
        }
        
        var updateResult = existingMunicipality.Value.Update(
            request.Code, request.Request.MunicipalityName, request.Request.MunicipalityStatus,
            request.Request.PouCode, request.Request.PouName, request.Request.OrpCode, request.Request.OrpName,
            request.Request.DistrictCode, request.Request.DistrictName, request.Request.VuscCode, request.Request.VuscName,
            request.Request.ShouldImportAddressLocation);
        if (updateResult.IsError)
        {
            logger.LogWarning("Cuzk - Object {ObjectName} cannot be updated because of error, MunicipalityId: {MunicipalityId}, " +
                              "ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}, Request: {@Request}", 
                nameof(Municipality), existingMunicipality.Value.Id, 
                updateResult.FirstError.Code, updateResult.FirstError.Description, request);
            return updateResult.Errors;
        }

        var municipalityUpdatedEvent = new MunicipalityUpdatedEvent(
            existingMunicipality.Value.Id,
            existingMunicipality.Value.Code,
            existingMunicipality.Value.Name,
            existingMunicipality.Value.Status,
            existingMunicipality.Value.PouCode,
            existingMunicipality.Value.PouName,
            existingMunicipality.Value.OrpCode,
            existingMunicipality.Value.OrpName,
            existingMunicipality.Value.DistrictCode,
            existingMunicipality.Value.DistrictName,
            existingMunicipality.Value.VuscCode,
            existingMunicipality.Value.VuscName,
            existingMunicipality.Value.ShouldImportAddressLocation,
            clock.UtcNowOffset);
        
        unitOfWork.AppendEvent(existingMunicipality.Value.Id, municipalityUpdatedEvent);
        
        logger.LogInformation("Cuzk - Object {ObjectName} with id: {Id} updated. Request: {@Request}", 
            nameof(Municipality), 
            existingMunicipality.Value.Id,
            request);
        
        return Result.Success;
    }
}