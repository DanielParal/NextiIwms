using ClosedXML.Excel;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.ImportsExports;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.ImportsExports.Exports;
using Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.ExportHandlers;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartners;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.ImportExport;

internal class PartnerExportXlsxHandler(ISender sender) : ExportXlsxBaseTemplateHandler<Partner, ExportType>, ISettingsExportHandler<ExportType>
{
    protected override ExportType ExportType { get;  } = ExportType.PartnerXlsx;
    protected override string[] Headers { get; } = PartnerHeader.ExpectedFileHeader;
    protected override string WorksheetName { get; } = "Data";
    protected override string FileNamePrefix { get; } = "PartneriExport";

    protected override async Task<List<Partner>> FetchDataAsync(CancellationToken cancellationToken)
    {
        var filteredResult = await sender.Send(new GetPartnersQuery(new BaseFilteringParams()), cancellationToken);
        return filteredResult.Data;
    }

    protected override void PopulateData(IXLWorksheet worksheet, List<Partner> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var rowCount = i + 2;
            worksheet.Cell(rowCount, PartnerHeader.Kod).Value = entities[i].Code;
            worksheet.Cell(rowCount, PartnerHeader.Jmeno).Value = entities[i].Name;
        }
    }
}