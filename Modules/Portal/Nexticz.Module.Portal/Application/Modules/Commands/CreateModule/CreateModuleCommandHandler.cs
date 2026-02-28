using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Portal.Application.Interfaces;
using Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

namespace Nexticz.Module.Portal.Application.Modules.Commands.CreateModule;

internal class CreateModuleCommandHandler(
    ILogger<CreateModuleCommandHandler> logger,
    IPortalReadOnlyEventStoreRepository readOnlyRepository,
    IPortalUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<CreateModuleCommand, ErrorOr<Domain.ModuleAggregate.Module>>
{
    public async Task<ErrorOr<Domain.ModuleAggregate.Module>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        var allModules = await readOnlyRepository.GetAllAsync<Domain.ModuleAggregate.Module>(cancellationToken);

        var moduleWithExistingName = allModules.FirstOrDefault(
            m => string.Equals(m.Name, request.Request.Name, StringComparison.InvariantCultureIgnoreCase));

        if (moduleWithExistingName is not null)
        {
            logger.LogWarning("PORTAL - Module cannot be created because name already exists, ModuleName: {ModuleName}", 
                request.Request.Name);
            return ModuleErrors.ValidationModuleWithNameAlreadyExists;
        }
        
        var moduleWithHighestSortNumber = allModules
            .OrderByDescending(m => m.SortOrder)
            .FirstOrDefault();
        
        var sortNumber = moduleWithHighestSortNumber?.SortOrder == null ? 
            1 : moduleWithHighestSortNumber.SortOrder + 1;
        
        var createdModule = Domain.ModuleAggregate.Module.CreateFrom(
            request.Request.Name,
            request.Request.Icon,
            request.Request.BaseUrl,
            request.Request.IsActive,
            sortNumber,
            clock.UtcNowOffset);

        if (createdModule.IsError)
        {
            logger.LogWarning("PORTAL - Module cannot be created because of error, ErrorCode: {ErrorCode}, ErrorDescription: {ErrorDescription}", 
                createdModule.FirstError.Code, createdModule.FirstError.Description);
            return createdModule.Errors;
        }
        
        var moduleCreatedEvent = new ModuleCreatedEvent(
            createdModule.Value.Id, createdModule.Value.Name,
            createdModule.Value.Icon, createdModule.Value.BaseUrl, 
            createdModule.Value.IsActive, createdModule.Value.SortOrder, 
            createdModule.Value.CreatedAt);
        
        unitOfWork.StartStream<ModuleCreatedEvent, Domain.ModuleAggregate.Module>(createdModule.Value.Id, moduleCreatedEvent);
        
        logger.LogInformation("PORTAL - Module created, ModuleId: {ModuleId}, ModuleName: {ModuleName}", 
            createdModule.Value.Id, createdModule.Value.Name);
        
        return createdModule;       
    }
}