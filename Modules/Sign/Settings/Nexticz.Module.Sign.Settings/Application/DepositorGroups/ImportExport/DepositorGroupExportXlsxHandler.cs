using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroups;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.ImportExport;

internal class DepositorGroupExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<DepositorGroup, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.DepositorGroupXlsx;
    protected override string[] Headers { get; } = DepositorGroupHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "SkupinyUkladateluExport";

    protected override async Task<List<DepositorGroup>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetDepositorGroupsQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<DepositorGroup> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var rowCount = i + 2;
            worksheet.Cell(rowCount, DepositorGroupHeader.Kod).Value = entities[i].Code;
            worksheet.Cell(rowCount, DepositorGroupHeader.Jmeno).Value = entities[i].Name;
        }
    }
}