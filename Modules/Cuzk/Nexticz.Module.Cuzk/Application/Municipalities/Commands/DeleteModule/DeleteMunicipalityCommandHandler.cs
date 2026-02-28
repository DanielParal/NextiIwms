using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Application.Interfaces;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate;
using Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

namespace Nexticz.Module.Cuzk.Application.Municipalities.Commands.DeleteModule;

internal class DeleteMunicipalityCommandHandler(
    ISender sender,
    ILogger<DeleteMunicipalityCommandHandler> logger,
    ICuzkUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<DeleteMunicipalityCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteMunicipalityCommand request, CancellationToken cancellationToken)
    {
        var existingMunicipality = await sender.Send(new GetMunicipalityByCodeQuery(request.Code), cancellationToken);
        
        if (existingMunicipality.IsError)
        {
            logger.LogInformation("Cuzk - Did not find object {ObjectName}. Nothing to delete. Request: {@Request}", 
                nameof(Municipality), request);
            return MunicipalityErrors.ValidationMunicipalityWithCodeDoesNotExist;
        }
        
        var moduleDeletedEvent = new MunicipalityDeletedEvent(
            existingMunicipality.Value.Id, existingMunicipality.Value.Code, clock.UtcNowOffset);
        unitOfWork.AppendEvent(existingMunicipality.Value.Id, moduleDeletedEvent);
        
        logger.LogInformation("Cuzk - Object {ObjectName} with id: {Id} deleted. Request: {@Request}", 
            nameof(Municipality), existingMunicipality.Value.Id, request);
        
        return Result.Success;       
    }
}