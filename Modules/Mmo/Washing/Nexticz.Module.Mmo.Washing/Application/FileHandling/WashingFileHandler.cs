using Nexticz.Module.Mmo.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;

namespace Nexticz.Module.Mmo.Washing.Application.FileHandling;

internal class WashingFileHandler(AssetsSettings assetsSettings) 
    : FileHandler(assetsSettings), IWashingFileHandler;