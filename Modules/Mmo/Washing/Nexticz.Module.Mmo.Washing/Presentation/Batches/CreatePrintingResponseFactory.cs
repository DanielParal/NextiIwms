using Nexticz.Module.Mmo.Washing.Contracts.Batches;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Mmo.Washing.Application.Batches.Commands.CreatePrinting;


namespace Nexticz.Module.Mmo.Washing.Presentation.Batches;

internal static class CreatePrintingResponseFactory
{
    public static CreatePrintingResponse Create(CreatePrintingCommandResponse createPrintingCommandResponse)
    {
        var printingContract = new PrintingContract(
            createPrintingCommandResponse.Printing.Id,
            createPrintingCommandResponse.Printing.BatchId,
            createPrintingCommandResponse.Printing.KitId,
            createPrintingCommandResponse.Printing.DatePrinted,
            (PrintingStatusContract)createPrintingCommandResponse.Printing.Status,
            (PrintingTypeContract)createPrintingCommandResponse.Printing.Type,
            createPrintingCommandResponse.Printing.FailureReason);

        var fileContract = createPrintingCommandResponse.PrintingFile is null ?
            null :
            new FileResponse(
                createPrintingCommandResponse.PrintingFile.ContentBytes, 
                createPrintingCommandResponse.PrintingFile.ContentType, 
                createPrintingCommandResponse.PrintingFile.FileName);
        
        return new CreatePrintingResponse(printingContract, fileContract);
    }
}