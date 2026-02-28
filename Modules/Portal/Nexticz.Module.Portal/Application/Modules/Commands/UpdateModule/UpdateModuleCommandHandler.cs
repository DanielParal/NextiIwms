using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Portal.Application.Interfaces;
using Nexticz.Module.Portal.Application.Modules.Queries.GetModuleById;
using Nexticz.Module.Portal.Application.Modules.Queries.GetModuleByName;
using Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

namespace Nexticz.Module.Portal.Application.Modules.Commands.UpdateModule;

internal class UpdateModuleCommandHandler(
    ISender sender,
    ILogger<UpdateModuleCommandHandler> logger,
    IPortalUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<UpdateModuleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var moduleToUpdate = await sender.Send(new GetModuleByIdQuery(request.Id), cancellationToken);
        
        if (moduleToUpdate.IsError)
        {
            logger.LogInformation("PORTAL - Did not find object {ObjectName} with id: {Id}. Nothing to update", 
                nameof(Module), request.Id);
            return ModuleErrors.ValidationModuleWithIdDoesNotExist;
        }
        
        var existingModuleWithName = await sender.Send(new GetModuleByNameQuery(request.Request.Name), cancellationToken);
        if (existingModuleWithName.HasValue() && existingModuleWithName.Value.Id != request.Id)
        {
            logger.LogWarning("PORTAL - object {ObjectName} with name: {Name} already exists. Existin object id: {ExistingModuleId}", 
                nameof(Module), request.Request.Name, existingModuleWithName.Value.Id);
            return ModuleErrors.ValidationModuleWithNameAlreadyExists;
        }
        
        var updateResult = moduleToUpdate.Value.Update(request.Request.Name, request.Request.Icon, request.Request.BaseUrl, request.Request.IsActive);
        if (updateResult.IsError)
        {
            logger.LogWarning("PORTAL - Module cannot be updated because of error, ModuleId: {ModuleId}, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                moduleToUpdate.Value.Id, updateResult.FirstError.Code, updateResult.FirstError.Description);
            return updateResult.Errors;
        }

        var moduleUpdatedEvent = new ModuleUpdatedEvent(
            moduleToUpdate.Value.Id,
            moduleToUpdate.Value.Name,
            moduleToUpdate.Value.Icon, 
            moduleToUpdate.Value.BaseUrl,
            moduleToUpdate.Value.IsActive, 
            clock.UtcNowOffset);
        
        unitOfWork.AppendEvent(moduleToUpdate.Value.Id, moduleUpdatedEvent);
        
        logger.LogInformation("PORTAL - Object {ObjectName} with id: {Id} updated, " +
                              "ModuleName: {ModuleName}, ModuleIcon: {ModuleIcon}, ModuleBaseUrl: {ModuleBaseUrl}, ModuleActive: {ModuleActive}", 
            nameof(Module), 
            moduleToUpdate.Value.Id,
            moduleToUpdate.Value.Name,
            moduleToUpdate.Value.Icon, 
            moduleToUpdate.Value.BaseUrl,
            moduleToUpdate.Value.IsActive);
        
        return Result.Success;
    }
}