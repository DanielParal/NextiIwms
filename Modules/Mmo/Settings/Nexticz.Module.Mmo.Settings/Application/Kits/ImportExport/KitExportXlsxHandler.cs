using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKits;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;
using Nexticz.Module.Mmo.Settings.Domain.KitAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.ImportExport;

internal class KitExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<Kit, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.KitXlsx;
    protected override string[] Headers { get; } = KitHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "KitExport";

    protected override async Task<List<Kit>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetKitsQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<Kit> entities)
    {
        var currentRow = 2;
        foreach (var kit in entities)
        {
            foreach (var packagingQuantity in kit.PackagingCodeQuantities)
            {
                worksheet.Cell(currentRow, KitHeader.CisloKitu).Value = kit.KitNumber;
                worksheet.Cell(currentRow, KitHeader.KodUkladatele).Value = kit.DepositorCode;
                worksheet.Cell(currentRow, KitHeader.KodTypuKitu).Value = kit.KitTypeCode;
                worksheet.Cell(currentRow, KitHeader.KodDefiniceSapuKitu).Value = kit.KitSapDefinitionCode;
                worksheet.Cell(currentRow, KitHeader.KodVyroby).Value = kit.ManufactureCode;
                worksheet.Cell(currentRow, KitHeader.KodUrcujicihoBaleni).Value = kit.DefiningPackagingCode;
                worksheet.Cell(currentRow, KitHeader.KodBaleni).Value = packagingQuantity.PackagingCode;
                worksheet.Cell(currentRow, KitHeader.PocetBaleniVKitu).Value = packagingQuantity.Quantity;
                worksheet.Cell(currentRow, KitHeader.Poznamka).Value = kit.Note;
                worksheet.Cell(currentRow, KitHeader.CasChladnuti).Value = kit.DryingTime;
                worksheet.Cell(currentRow, KitHeader.ObsahujeBaliciPredpis).Value = kit.HasKitInstructionFile ? "ANO" : "NE";

                currentRow++;
            }
        }
    }
}