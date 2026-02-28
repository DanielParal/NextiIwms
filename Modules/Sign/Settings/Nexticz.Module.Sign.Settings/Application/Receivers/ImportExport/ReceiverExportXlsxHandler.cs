using ClosedXML.Excel;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.ImportsExports;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceivers;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.ImportExport;

internal class ReceiverExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<Receiver, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.ReceiverXlsx;
    protected override string[] Headers { get; } = ReceiverHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "PrijemciExport";

    protected override async Task<List<Receiver>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetReceiversQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<Receiver> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var rowCount = i + 2;
            worksheet.Cell(rowCount, ReceiverHeader.Kod).Value = entities[i].Code;
            worksheet.Cell(rowCount, ReceiverHeader.Jmeno).Value = entities[i].Name;
            worksheet.Cell(rowCount, ReceiverHeader.KodPartnera).Value = entities[i].PartnerCode;
        }
    }
}