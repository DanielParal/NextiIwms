using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.PrintingUtils;

namespace Nexticz.Module.Mmo.Washing.Application.Printings;

internal class WashingPrintHandler(
    ILogger<WashingPrintHandler> logger) : PrintHandler(logger), IWashingPrintHandler
{
}