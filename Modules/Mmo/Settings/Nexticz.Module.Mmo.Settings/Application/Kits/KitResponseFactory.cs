using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingsByCodes;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypesByCodes;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits;

internal class KitResponseFactory(ISender sender)
{
    public async Task<KitResponse> CreateAsync(Kit kit, CancellationToken cancellationToken)
    {
        var distinctPackagingCodes = kit.PackagingCodeQuantities.Select(x => x.PackagingCode).Distinct().ToArray();
        var packagingTypeNamesDictionary = await GetPackagingTypeNamesDictionaryAsync(distinctPackagingCodes, cancellationToken);
        var kitResponse = Create(kit, packagingTypeNamesDictionary);
        return kitResponse;
    }
    
    public async Task<KitResponse[]> CreateAsync(Kit[] kits, CancellationToken cancellationToken)
    {
        var distinctPackagingCodes = kits.SelectMany(x => x.PackagingCodeQuantities).Select(x => x.PackagingCode).Distinct().ToArray();
        var packagingTypeNamesDictionary = await GetPackagingTypeNamesDictionaryAsync(distinctPackagingCodes, cancellationToken);
        
        var kitResponses = kits.Select(x => Create(x, packagingTypeNamesDictionary)).ToArray();
        return kitResponses;
    }

    private static KitResponse Create(Kit kit, Dictionary<string, string> packagingTypeNamesDictionary)
    {
        return new KitResponse(
            kit.Id,
            kit.Code,
            kit.KitTypeCode,
            kit.KitSapDefinitionCode,
            kit.DepositorCode,
            kit.ManufactureCode,
            kit.KitNumber,
            kit.Note,
            kit.DefiningPackagingCode,
            kit.DryingTime,
            kit.HasKitInstructionFile,
            kit.PackagingCodeQuantities
                .Select(x => new PackagingQuantityContract(x.PackagingCode, packagingTypeNamesDictionary[x.PackagingCode], x.Quantity))
                .ToArray(),
            kit.SpecialInformationSchedules
                .Select(x => new SpecialInformationScheduleContract(x.Id, x.StartDate, x.EndDate))
                .ToArray()
        );
    }

    private async Task<Dictionary<string, string>> GetPackagingTypeNamesDictionaryAsync(
        string[] distinctPackagingCodes,
        CancellationToken cancellationToken)
    {
        var packagings = await sender.Send(new GetPackagingsByCodesQuery(distinctPackagingCodes), cancellationToken);
        var packagingTypeCodes = packagings.Select(x => x.PackagingTypeCode).Distinct().ToArray();
        var packagingTypes = await sender.Send(new GetPackagingTypesByCodesQuery(packagingTypeCodes), cancellationToken);
        
        var packagingTypesDictionary = packagingTypes.ToDictionary(pt => pt.Code, pt => pt.Name);

        var packagingTypeNamesDictionary = packagings
            .Where(p => packagingTypesDictionary.ContainsKey(p.PackagingTypeCode))
            .ToDictionary(
                p => p.Code,
                p => packagingTypesDictionary[p.PackagingTypeCode]
            );
        
        return packagingTypeNamesDictionary;
    }
}