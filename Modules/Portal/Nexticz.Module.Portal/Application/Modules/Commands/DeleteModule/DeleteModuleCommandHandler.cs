using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Portal.Application.Interfaces;
using Nexticz.Module.Portal.Application.Modules.Queries.GetModuleById;
using Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

namespace Nexticz.Module.Portal.Application.Modules.Commands.DeleteModule;

internal class DeleteModuleCommandHandler(
    ISender sender,
    ILogger<DeleteModuleCommandHandler> logger,
    IPortalUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<DeleteModuleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        var existingModule = await sender.Send(new GetModuleByIdQuery(request.Id), cancellationToken);
        
        if (existingModule.IsError)
        {
            logger.LogInformation("PORTAL - Did not find object {ObjectName} with id: {Id}. Nothing to delete.", 
                nameof(Module), request.Id);
            return ModuleErrors.ValidationModuleWithIdDoesNotExist;
        }
        
        var moduleDeletedEvent = new ModuleDeletedEvent(existingModule.Value.Id, clock.UtcNowOffset);
        unitOfWork.AppendEvent(existingModule.Value.Id, moduleDeletedEvent);
        
        logger.LogInformation("PORTAL - Object {ObjectName} with id: {Id} deleted.", 
            nameof(Module), request.Id);
        
        return Result.Success;       
    }
}