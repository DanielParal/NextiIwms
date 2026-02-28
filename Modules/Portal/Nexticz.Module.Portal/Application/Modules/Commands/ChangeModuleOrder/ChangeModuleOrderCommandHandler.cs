using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Portal.Application.Interfaces;
using Nexticz.Module.Portal.Domain.ModuleAggregate.Events;

namespace Nexticz.Module.Portal.Application.Modules.Commands.ChangeModuleOrder;

internal class ChangeModuleOrderCommandHandler(
    IPortalReadOnlyEventStoreRepository readOnlyRepository,
    ILogger<ChangeModuleOrderCommandHandler> logger,
    IPortalUnitOfWork unitOfWork,
    IClock clock) : IRequestHandler<ChangeModuleOrderCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ChangeModuleOrderCommand request, CancellationToken cancellationToken)
    {
        var allModules = await readOnlyRepository.GetAllAsync<Domain.ModuleAggregate.Module>(cancellationToken);
        var moduleToChange = allModules.FirstOrDefault(x => x.Id == request.Id);
        
        if (moduleToChange is null)
        {
            logger.LogInformation("PORTAL - Did not find object {ObjectName} with id: {Id}. Cannot change order", 
                nameof(Module), request.Id);
            return ModuleErrors.ValidationModuleWithIdDoesNotExist;
        }

        if (allModules.Count < request.ToOrder)
        {
            logger.LogWarning("PORTAL - cannot move it to the position higher than the number of modules. ModuleId: {ModuleId}, ToOrder: {ToOrder}",
                moduleToChange.Id, request.ToOrder);
            return ModuleErrors.ValidationCannotMoveItHigherThanNumberOfModules;
        }
        
        var (modulesToMove, direction) = FilterModulesToMove(allModules, moduleToChange.SortOrder, request.ToOrder);

        var sortOrderDiff = direction == CurrentModuleChangeDirection.Up ? -1 : 1;
        var dateTimeOffset = clock.UtcNowOffset;
        
        foreach (var module in modulesToMove)
        {
            var newSortOrder = module.SortOrder + sortOrderDiff;
            var moduleOrderChangedEvent = 
                new ModuleOrderChangedEvent(module.Id, newSortOrder, dateTimeOffset);
            unitOfWork.AppendEvent(module.Id, moduleOrderChangedEvent);
            logger.LogInformation("PORTAL - module change its order. ModuleId: {ModuleId}, NewOrder: {NewOrder}",
                module.Id, newSortOrder);
        }
        
        var currentModuleOrderChangedEvent = new ModuleOrderChangedEvent(moduleToChange.Id, request.ToOrder, dateTimeOffset);
        unitOfWork.AppendEvent(moduleToChange.Id, currentModuleOrderChangedEvent);
        logger.LogInformation("PORTAL - current module change its order. ModuleId: {ModuleId}, NewOrder: {NewOrder}",
            moduleToChange.Id, request.ToOrder);
        
        return Result.Success;
    }

    private static (List<Domain.ModuleAggregate.Module> ModulesToMove, CurrentModuleChangeDirection Direction) FilterModulesToMove(
        IReadOnlyList<Domain.ModuleAggregate.Module> modules, int currentModuleOrder, int toOrder)
    {
        var direction = currentModuleOrder > toOrder ? CurrentModuleChangeDirection.Down : CurrentModuleChangeDirection.Up;
        
        if (direction == CurrentModuleChangeDirection.Down)
        {
            return (modules.Where(x => x.SortOrder >= toOrder && x.SortOrder < currentModuleOrder).ToList(), direction);
        }
        
        return (modules.Where(x => x.SortOrder <= toOrder && x.SortOrder > currentModuleOrder).ToList(), direction);
    }
    
    private enum CurrentModuleChangeDirection
    {
        Down,
        Up
    }
}