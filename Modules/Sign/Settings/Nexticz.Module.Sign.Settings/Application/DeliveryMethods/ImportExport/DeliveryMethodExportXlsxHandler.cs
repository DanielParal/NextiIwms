using ClosedXML.Excel;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethods;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Application.Partners.ImportExport;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.ImportExport;

internal class DeliveryMethodExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<DeliveryMethod, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.DeliveryMethodXlsx;
    protected override string[] Headers { get; } = DeliveryMethodHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "ZpusobDodaniExport";

    protected override async Task<List<DeliveryMethod>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetDeliveryMethodsQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<DeliveryMethod> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var rowCount = i + 2;
            worksheet.Cell(rowCount, PartnerHeader.Kod).Value = entities[i].Code;
            worksheet.Cell(rowCount, PartnerHeader.Jmeno).Value = entities[i].Name;
            worksheet.Cell(rowCount, DeliveryMethodHeader.PocetKopiiProNakladniList).Value = entities[i].LoadingDocumentPrintCopiesCount;
            worksheet.Cell(rowCount, DeliveryMethodHeader.PocetKopiiProDodaciList).Value = entities[i].DeliveryDocumentPrintCopiesCount;
        }
    }
}