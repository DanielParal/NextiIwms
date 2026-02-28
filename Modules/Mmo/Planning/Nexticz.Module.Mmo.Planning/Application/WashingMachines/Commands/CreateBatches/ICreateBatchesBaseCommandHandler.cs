using ErrorOr;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Domain.WashingMachineAggregate;

namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches;

internal interface ICreateBatchesBaseCommandHandler
{
    Task<ErrorOr<KitResponse>> GetKitResponseByCodeAsync(string kitCode, CancellationToken cancellationToken);
    
    Task<ErrorOr<PackagingResponse>> GetPackagingResponseByCodeAsync(string packagingCode,
        CancellationToken cancellationToken);

    ErrorOr<Success> IsPackagingInKit(PackagingResponse packagingFromSettings, KitResponse kitFromSettings);

    Task<WashingMachineResponse[]> GetFilteredWashingMachineResponsesByPackagingCodeAsync(string packagingCode,
        string? sisterPackagingCode, CancellationToken cancellationToken);

    Task<ErrorOr<WashingMachine>> GetWashingMachineByLineQueueCodeAsync(
        string lineQueueCode, WashingMachineResponse[] filteredWashingMachineResponses,
        CancellationToken cancellationToken);

    Task<int> GetSpaceBetweenPackagingsConstantAsync(CancellationToken cancellationToken);

    ErrorOr<TimeSpan> CalculateOptimalWashingTime(
        KitResponse kit,
        PackagingResponse packaging,
        WashingMachineResponse washingMachineResponse,
        int spaceBetweenPackagings);
}