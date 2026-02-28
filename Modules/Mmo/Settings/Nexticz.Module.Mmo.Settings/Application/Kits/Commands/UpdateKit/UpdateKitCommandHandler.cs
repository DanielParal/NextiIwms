using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByCodes;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationsByIds;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UpdateKit;

internal class UpdateKitCommandHandler (
    ILogger<UpdateKitCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler fileHandler,
    ISender sender,
    IClock clock)
    : IRequestHandler<UpdateKitCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateKitCommand request, CancellationToken cancellationToken)
    {
        var kit = await sender.Send(new GetKitByCodeQuery(request.Code), cancellationToken);
        
        var validationResult = await IsValidAsync(kit, request, cancellationToken);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var packagingCodeQuantities = request.UpdateKitRequest.PackagingQuantities.Select(x => new PackagingCodeQuantity(x.PackagingCode, x.Quantity)).ToArray();
        var specialInformationSchedules = 
            request.UpdateKitRequest.SpecialInformationSchedules
                .Select(x => 
                new SpecialInformationSchedule(
                    x.Id, 
                    clock.NormalizeToNoonUtc(x.StartDate), 
                    clock.NormalizeToNoonUtc(x.EndDate)))
                .ToArray();
        
        var kitInstructionPdf = await fileHandler.GetKitInstructionPdfAsync(request.Code, cancellationToken);

        var kitUpdatedEvent = new KitUpdatedEvent(
            kit.Value.Id, request.Code, request.UpdateKitRequest.ManufactureCode, request.UpdateKitRequest.KitSapDefinitionCode,
            request.UpdateKitRequest.Note, request.UpdateKitRequest.DefiningPackagingCode, 
            request.UpdateKitRequest.DryingTime, 
            kitInstructionPdf is not null,
            packagingCodeQuantities,
            specialInformationSchedules
        );
        
        unitOfWork.AppendEvent(kit.Value.Id, kitUpdatedEvent);

        return Result.Updated;
    }

    private async Task<ErrorOr<Success>> IsValidAsync(ErrorOr<Kit> errorOrKit, UpdateKitCommand request, CancellationToken cancellationToken)
    {
        if (!errorOrKit.HasValue())
        {
            logger.LogInformation("Did not find object {ObjectName} with Code: {Code}. Nothing to update.", nameof(Kit), request.Code);
            return errorOrKit.Errors;
        }
        
        if (HasOverlappingSpecialInformationSchedules(request.UpdateKitRequest.SpecialInformationSchedules))
        {
            logger.LogInformation("Object {ObjectName} has overlapping schedules.", nameof(SpecialInformation));
            return KitErrors.ValidationOverlappingSpecialInformations;
        }
        
        var specialInformations = await GetSpecialInformationsByIds(request.UpdateKitRequest.SpecialInformationSchedules, cancellationToken);
        if (specialInformations.Length != request.UpdateKitRequest.SpecialInformationSchedules.Length)
        {
            
            logger.LogInformation("Object {ObjectName} - not all special informations are in the db. " +
                                  "Special information ids from db: {IdsFromDb}, Special information ids from request: {IdsFromRequest}.", 
                nameof(SpecialInformation), 
                string.Join(", ", specialInformations.Select(x => x.Id)), 
                string.Join(", ", request.UpdateKitRequest.SpecialInformationSchedules.Select(x => x.Id)));
            return KitErrors.ValidationNonExistingSpecialInformations;
        }
        
        var manufacture = await sender.Send(new GetManufactureByCodeQuery(request.UpdateKitRequest.ManufactureCode), cancellationToken);
        if (manufacture.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(Manufacture), request.UpdateKitRequest.ManufactureCode);
            return KitErrors.ValidationManufactureDoesNotExist;
        }
        
        var kitSapDefinitionCode = await sender.Send(new GetKitSapDefinitionByCodeQuery(request.UpdateKitRequest.KitSapDefinitionCode), cancellationToken);
        if (kitSapDefinitionCode.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(KitSapDefinition), request.UpdateKitRequest.KitSapDefinitionCode);
            return KitErrors.ValidationKitSapDefinitionDoesNotExist;
        }
        
        var packagingCodesFromRequest = request.UpdateKitRequest.PackagingQuantities.Select(x => x.PackagingCode.ToUpperInvariant()).ToArray();
        var existingPackagings = await sender.Send(new GetPackagingsByCodesQuery(packagingCodesFromRequest), cancellationToken);
        var existingPackagingCodes = existingPackagings.Select(p => p.Code).ToHashSet();
        var missingPackagingCodes = packagingCodesFromRequest.Where(code => !existingPackagingCodes.Contains(code)).ToArray();
        if (missingPackagingCodes.Length != 0)
        {
            var missingPackagingCodesComaSeparated = string.Join(", ", missingPackagingCodes);
            logger.LogInformation("The following packaging unique codes are missing: {MissingCodes}. Nothing to update.", string.Join(", ", missingPackagingCodesComaSeparated));
            return KitErrors.ValidationMissingPackagingCodes(missingPackagingCodesComaSeparated);
        }

        if (!packagingCodesFromRequest.Contains(
                request.UpdateKitRequest.DefiningPackagingCode, StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("DefiningPackagingCode: {DefiningPackagingCode} is not included in PackagingCodes. Nothing to update.", request.UpdateKitRequest.DefiningPackagingCode);
            return KitErrors.ValidationDefiningPackagingCodeNotIncludedInPackagingCodes;
        }

        return Result.Success;
    }
    
    private async Task<SpecialInformation[]> GetSpecialInformationsByIds(SpecialInformationScheduleContract[] schedules, CancellationToken cancellationToken)
    {
        if (schedules.Length == 0)
            return [];
        
        var existingItems = await sender.Send(new GetSpecialInformationsByIdsQuery(schedules.Select(x => x.Id).ToArray()), cancellationToken);
        return existingItems;
    }
    
    private static bool HasOverlappingSpecialInformationSchedules(SpecialInformationScheduleContract[] schedules)
    {
        var sortedSchedules = schedules.OrderBy(s => s.StartDate).ToList();

        for (var i = 0; i < sortedSchedules.Count - 1; i++)
        {
            var current = sortedSchedules[i];
            
            if (i + 1 >= sortedSchedules.Count)
                continue;
        
            var next = sortedSchedules[i + 1];
        
            if (current.EndDate > next.StartDate)
                return true;
        }
        
        return false;
    }
}