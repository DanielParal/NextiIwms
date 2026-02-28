using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;
using Nexticz.Module.Mmo.Settings.Application.FileHandling;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByCodes;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationsByIds;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using KitAggregate_PackagingCodeQuantity = Nexticz.Module.Mmo.Settings.Domain.KitAggregate.PackagingCodeQuantity;
using PackagingCodeQuantity = Nexticz.Module.Mmo.Settings.Domain.KitAggregate.PackagingCodeQuantity;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.CreateKit;

internal class CreateKitCommandHandler (
    ILogger<CreateKitCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISettingsFileHandler fileHandler,
    ISender sender)
    : IRequestHandler<CreateKitCommand, ErrorOr<Kit>>
{
    public async Task<ErrorOr<Kit>> Handle(CreateKitCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await IsValidAsync(request.CreateKitRequest, cancellationToken);
        
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var uniqueCode = request.CreateKitRequest.DepositorCode + request.CreateKitRequest.KitTypeCode + request.CreateKitRequest.KitNumber;
        var packagingCodeQuantities = request.CreateKitRequest.PackagingQuantities.Select(x => new KitAggregate_PackagingCodeQuantity(x.PackagingCode, x.Quantity)).ToArray();
        var specialInformationSchedules = request.CreateKitRequest.SpecialInformationSchedules.Select(x => new SpecialInformationSchedule(x.Id, x.StartDate, x.EndDate)).ToArray();
        var kitInstructionPdf = await fileHandler.GetKitInstructionPdfAsync(uniqueCode, cancellationToken);
        
        var kit = new Kit(
            uniqueCode, request.CreateKitRequest.KitTypeCode, request.CreateKitRequest.KitSapDefinitionCode, request.CreateKitRequest.DepositorCode, 
            request.CreateKitRequest.ManufactureCode, request.CreateKitRequest.KitNumber, request.CreateKitRequest.Note, 
            request.CreateKitRequest.DefiningPackagingCode, request.CreateKitRequest.DryingTime,
            kitInstructionPdf is not null, packagingCodeQuantities, specialInformationSchedules
        );

        var kitCreatedEvent = new KitCreatedEvent(
            kit.Id, kit.Code, kit.KitTypeCode, kit.KitSapDefinitionCode, kit.DepositorCode, 
            kit.ManufactureCode, kit.KitNumber, kit.Note, 
            kit.DefiningPackagingCode, kit.DryingTime,
            kit.HasKitInstructionFile, kit.PackagingCodeQuantities, kit.SpecialInformationSchedules
        );
        
        unitOfWork.StartStream<KitCreatedEvent, Kit>(kit.Id, kitCreatedEvent);

        return kit;
    }

    private async Task<ErrorOr<Success>> IsValidAsync(CreateKitRequest request, CancellationToken cancellationToken)
    {
        if (HasOverlappingSpecialInformationSchedules(request.SpecialInformationSchedules))
        {
            logger.LogInformation("Object {ObjectName} has overlapping schedules.", nameof(SpecialInformation));
            return KitErrors.ValidationOverlappingSpecialInformations;
        }
        
        var specialInformations = await GetSpecialInformationsByIds(request.SpecialInformationSchedules, cancellationToken);
        if (specialInformations.Length != request.SpecialInformationSchedules.Length)
        {
            
            logger.LogInformation("Object {ObjectName} - not all special informations are in the db. " +
                                  "Special information ids from db: {IdsFromDb}, Special information ids from request: {IdsFromRequest}.", 
                nameof(SpecialInformation), 
                string.Join(", ", specialInformations.Select(x => x.Id)), 
                string.Join(", ", request.SpecialInformationSchedules.Select(x => x.Id)));
            return KitErrors.ValidationNonExistingSpecialInformations;
        }
        
        var depositor = await sender.Send(new GetDepositorByCodeQuery(request.DepositorCode), cancellationToken);
        if (depositor.IsError)
        {
            logger.LogInformation("Object {ObjectName} with code: {Code} already exists. Nothing to create.", nameof(Depositor), request.DepositorCode);
            return KitErrors.ValidationDepositorDoesNotExist;
        }
        
        var kitType = await sender.Send(new GetKitTypeByCodeQuery(request.KitTypeCode), cancellationToken);
        if (kitType.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(KitType), request.KitTypeCode);
            return KitErrors.ValidationKitTypeDoesNotExist;
        }
        
        var kitSapDefinition = await sender.Send(new GetKitSapDefinitionByCodeQuery(request.KitSapDefinitionCode), cancellationToken);
        if (kitSapDefinition.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(KitSapDefinition), request.KitSapDefinitionCode);
            return KitErrors.ValidationKitSapDefinitionDoesNotExist;
        }
        
        var manufacture = await sender.Send(new GetManufactureByCodeQuery(request.ManufactureCode), cancellationToken);
        if (manufacture.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with code: {Code}. Nothing to update.", nameof(Manufacture), request.ManufactureCode);
            return KitErrors.ValidationManufactureDoesNotExist;
        }

        var uniqueCode = request.DepositorCode + request.KitTypeCode + request.KitNumber;
        var existingKit = await sender.Send(new GetKitByCodeQuery(uniqueCode), cancellationToken);
        if (existingKit.HasValue())
        {
            logger.LogInformation("Combination of depositor code: {DepositorCode} and customer number: {CustomerNumber} already exists. Nothing to create.", 
                request.DepositorCode, request.KitNumber);
            return KitErrors.ValidationCombinationDepositorAndKitNumberExist;
        }
        
        var packagingCodesFromRequest = request.PackagingQuantities.Select(x => x.PackagingCode.ToUpperInvariant()).ToArray();
        var existingPackagings = await sender.Send(new GetPackagingsByCodesQuery(packagingCodesFromRequest), cancellationToken);
        var existingPackagingCodes = existingPackagings.Select(p => p.Code).ToHashSet();
        var missingPackagingCodes = packagingCodesFromRequest.Where(code => !existingPackagingCodes.Contains(code)).ToArray();
        if (missingPackagingCodes.Length != 0)
        {
            var missingPackagingCodesComaSeparated = string.Join(", ", missingPackagingCodes);
            logger.LogInformation("The following packaging unique codes are missing: {MissingCodes}. Nothing to create.", string.Join(", ", missingPackagingCodesComaSeparated));
            return KitErrors.ValidationMissingPackagingCodes(missingPackagingCodesComaSeparated);
        }

        if (!packagingCodesFromRequest.Contains(
                request.DefiningPackagingCode, StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("DefiningPackagingCode: {DefiningPackagingCode} is not included in PackagingCodes. Nothing to create.", request.DefiningPackagingCode);
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