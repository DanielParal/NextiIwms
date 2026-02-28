using Nexticz.Module.Mmo.SharedKernel.Efficiencies;
using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;
using Nexticz.Module.Mmo.Washing.Domain.SpecialInformationEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches;

internal static class BatchResponseFactory
{
    public static BatchResponse Create(Batch batch, bool isSpecialInformationConfirmationNeeded, bool shouldBeLogout, 
        string? reasonForLogout, bool canCompleteKit, string? reasonWhyKitCannotBeCompleted, bool isHelpNeeded)
    {
        var kitWashCyclesContract =
            batch.KitWashCycles
                .Select(KitWashCycleContractFactory.Create)
                .ToArray();

        var printings = batch.Printings
            .Select(x =>
                new PrintingContract(x.Id, x.BatchId, x.KitId, x.DatePrinted, (PrintingStatusContract)x.Status,
                    (PrintingTypeContract)x.Type, x.FailureReason))
            .ToArray();

        var specialInformationContract = GetSpecialInformationContract(batch.SpecialInformation);
        var averageEfficiency = kitWashCyclesContract.Length == 0 ? 0 : kitWashCyclesContract.Average(x => (double)x.Efficiency!);
        var batchContract = new BatchResponse(
            batch.Id,
            batch.SisterBatchId,
            batch.KitCode,
            batch.PackagingCode,
            batch.DefiningPackagingCode,
            batch.OptimalKitDuration,
            kitWashCyclesContract.Length,
            batch.PlannedKitsCount,
            EfficiencyFormatter.EfficiencyToPercentageString(averageEfficiency),
            kitWashCyclesContract,
            printings,
            isSpecialInformationConfirmationNeeded,
            specialInformationContract,
            shouldBeLogout,
            reasonForLogout,
            canCompleteKit,
            reasonWhyKitCannotBeCompleted,
            batch.WashingMachineCode,
            isHelpNeeded);
        
        return batchContract;
    }
    
    private static SpecialInformationContract? GetSpecialInformationContract(SpecialInformation? specialInformation)
    {
        if (specialInformation is null)
            return null;

        return new SpecialInformationContract(specialInformation.Id, specialInformation.Title,
            specialInformation.Description, specialInformation.HasFile);
    }
}