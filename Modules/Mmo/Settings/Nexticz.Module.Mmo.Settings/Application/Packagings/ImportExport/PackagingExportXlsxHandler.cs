using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports;
using Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagings;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.ImportExport;

internal class PackagingExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<Packaging, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.PackagingXlsx;
    protected override string[] Headers { get; } = PackagingHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "ObalyExport";

    protected override async Task<List<Packaging>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetPackagingsQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<Packaging> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var rowCount = i + 2;
            worksheet.Cell(rowCount, PackagingHeader.ZakaznickeCisloObalu).Value = entities[i].CustomerNumber;
            worksheet.Cell(rowCount, PackagingHeader.KodUkladatele).Value = entities[i].DepositorCode;
            worksheet.Cell(rowCount, PackagingHeader.KodTypuObalu).Value = entities[i].PackagingTypeCode;
            worksheet.Cell(rowCount, PackagingHeader.KodObehovostiObalu).Value = entities[i].PackagingCirculationCode;
            worksheet.Cell(rowCount, PackagingHeader.NutnoPrat).Value = entities[i].MustBeWashed ? "ANO" : "NE";
            worksheet.Cell(rowCount, PackagingHeader.NazevObalu).Value = entities[i].Name;
            worksheet.Cell(rowCount, PackagingHeader.Hloubka).Value = entities[i].Dimensions.Depth;
            worksheet.Cell(rowCount, PackagingHeader.Sirka).Value = entities[i].Dimensions.Width;
            worksheet.Cell(rowCount, PackagingHeader.Vyska).Value = entities[i].Dimensions.Height;
            worksheet.Cell(rowCount, PackagingHeader.Hmotnost).Value = entities[i].Weight;
            worksheet.Cell(rowCount, PackagingHeader.RychlostiMycek).Value = string.Join(";", entities[i].WashingMachineSpeeds.Select(x => $"{x.WashingMachineCode}:{x.Speed}")) ;
        }
    }
}