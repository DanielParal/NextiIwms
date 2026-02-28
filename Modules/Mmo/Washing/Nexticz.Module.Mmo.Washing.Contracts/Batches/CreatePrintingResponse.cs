using System.ComponentModel.DataAnnotations;
using Nexticz.Lib.Shared.FileHandling.Models;


namespace Nexticz.Module.Mmo.Washing.Contracts.Batches;

public record CreatePrintingResponse(
    [property: Required] PrintingContract Printing,
    FileResponse? File);