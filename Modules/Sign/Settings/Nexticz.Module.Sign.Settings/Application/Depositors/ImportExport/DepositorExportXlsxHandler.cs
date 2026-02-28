using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositors;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.ImportExport;

internal class DepositorExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<Depositor, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.DepositorXlsx;
    protected override string[] Headers { get; } = DepositorHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "UkladateleExport";

    protected override async Task<List<Depositor>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetDepositorsQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<Depositor> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var rowCount = i + 2;
            worksheet.Cell(rowCount, DepositorHeader.Kod).Value = entities[i].Code;
            worksheet.Cell(rowCount, DepositorHeader.Jmeno).Value = entities[i].Name;
            worksheet.Cell(rowCount, DepositorHeader.KodSkupinyUkladatelu).Value = entities[i].DepositorGroupCode;
            worksheet.Cell(rowCount, DepositorHeader.KodSablonyDodacihoListu).Value = entities[i].DeliveryTemplateCode;
            worksheet.Cell(rowCount, DepositorHeader.KodSablonyNakladnihoListu).Value = entities[i].LoadingTemplateCode;
        }
    }
}