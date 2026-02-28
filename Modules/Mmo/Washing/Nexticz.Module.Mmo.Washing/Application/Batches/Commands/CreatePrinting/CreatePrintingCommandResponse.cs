using Nexticz.Module.Mmo.Washing.Application.Printings;
using Nexticz.Module.Mmo.Washing.Domain.PrintingEntity;

namespace Nexticz.Module.Mmo.Washing.Application.Batches.Commands.CreatePrinting;

internal record CreatePrintingCommandResponse(
    Printing Printing,
    PrintingFile? PrintingFile);