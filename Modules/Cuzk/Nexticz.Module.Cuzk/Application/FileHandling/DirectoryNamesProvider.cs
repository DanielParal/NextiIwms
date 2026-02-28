using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.FileHandling;

internal static class DirectoryNamesProvider
{
    private const string ModuleName = "CUZK";
    public static string RequestedImportsFolder(AssetsSettings assetsSettings) => $"{assetsSettings.BaseFolder}/{ModuleName}/RequestedImports";
    public static string CreateFileName(Guid importId, string fileExtension, ImportType type, IClock clock) => $"{clock.UtcNowOffset:yyyyMMdd_hhmmss}_{type.ToString()}_{importId}{fileExtension}";
}