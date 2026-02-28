using ClosedXML.Excel;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents.MetadataCalculators;

namespace Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;

public class PdaReaderRawEventsMyStockXlsxHandler(MyStockHandlersCalculators myStockHandlersCalculators)
    : IPdaReaderRawEventsHandler
{
    private static readonly string[] ExpectedFileHeader =
    [
        "Kód licence",
        "Kód akce",
        "Datum a čas",
        "Kód osoby",
        "Kód ukladatele",
        "Kód skupiny ukladatele",
        "Kód skladu",
        "Kód skladu externího systému",
        "Kód sortimentu",
        "Externí kód sortimentu",
        "Kód partnera",
        "Kód partnera externího systému",
        "Kód umístění",
        "Vychystávací umístění",
        "Primární doklad",
        "Upravil",
        "Upraveno",
        "Založil",
        "Založeno"
    ];

    public async Task<ErrorOr<List<LoadedActivity>>> Handle(PdaReaderRawEvents pdaReaderRawEvents,
        CancellationToken cancellationToken)
    {
        if (!CheckCorrectDataFormat(pdaReaderRawEvents.Data)) return Error.Validation("File is not in correct format");

        return await CreateLoadedActivitiesFromData(pdaReaderRawEvents.Data);
    }

    public bool CanHandle(PdaReaderRawEventsSource pdaReaderRawEventsSource)
    {
        return pdaReaderRawEventsSource == PdaReaderRawEventsSource.MyStockXlsx;
    }

    private async Task<List<LoadedActivity>> CreateLoadedActivitiesFromData(object data)
    {
        if (data is not IFormFile file) return [];

        List<LoadedActivity> loadedActivities = [];

        await myStockHandlersCalculators.InitializeAsync();

        await using var stream = file.OpenReadStream();

        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheet(1);

        var lastRowUsed = worksheet.LastRowUsed()!.RowNumber();

        for (var rowIndex = 2; rowIndex <= lastRowUsed; rowIndex++)
        {
            var currentRow = worksheet.Row(rowIndex);

            var systemType = currentRow.Cell(1).GetValue<string>();
            var activityCode = currentRow.Cell(2).GetValue<string>();
            ;
            var start = DateTime.Parse(currentRow.Cell(3).GetValue<string>());
            var workerCode = currentRow.Cell(4).GetValue<string>();
            var depositorCode = currentRow.Cell(5).GetValue<string>();
            var depositorGroupCode = currentRow.Cell(6).GetValue<string>();
            var assortmentCode = currentRow.Cell(9).GetValue<string>();
            var partnerCode = currentRow.Cell(11).GetValue<string>();
            var pickingPlaceLow = currentRow.Cell(14).GetValue<int>();

            myStockHandlersCalculators.InitializeItemProperties(
                activityCode,
                depositorCode,
                systemType,
                depositorGroupCode,
                assortmentCode,
                partnerCode,
                IsPickingPlaceHigh(pickingPlaceLow)
            );

            var created = DateTime.Now;
            var centerCode = myStockHandlersCalculators.GetCenterCode();
            var systemActivityType = myStockHandlersCalculators.GetSystemActivityType();
            var activityType = myStockHandlersCalculators.GetActivityType();
            const ActivitySource activitySource = ActivitySource.MyStock;
            const ActivityState activityState = ActivityState.Copied;

            loadedActivities.Add(
                new LoadedActivity(
                    created,
                    start,
                    workerCode,
                    centerCode,
                    activityCode,
                    depositorCode,
                    depositorGroupCode,
                    systemActivityType,
                    activityType,
                    activitySource,
                    activityState,
                    IsPickingPlaceHigh(pickingPlaceLow)
                )
                {
                    ActivityCutOff = myStockHandlersCalculators.GetActivityCutOff(),
                    Unit = myStockHandlersCalculators.GetUnit(),
                    Coefficient = myStockHandlersCalculators.GetCoefficient(),
                    Idd = 0,
                    Idi = 0,
                    Idp = 0,
                    Idt = 0,
                    ActivityName = myStockHandlersCalculators.GetActivityName(),
                    PDoklad = "",
                    SortKod = "",
                    LicenceKod = ""
                }
            );
        }

        return loadedActivities;
    }

    private static bool IsPickingPlaceHigh(int pickingPlaceLow)
    {
        return pickingPlaceLow == 0;
    }

    private static bool CheckCorrectDataFormat(object data)
    {
        return IsDataObjectIFormFile(data) && HasFileCorrectStructure(data);
    }

    private static bool HasFileCorrectStructure(object data)
    {
        if (data is not IFormFile file) return false;

        using var stream = file.OpenReadStream();

        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheet(1);
        var row = worksheet.Row(1);

        for (var i = 0; i < ExpectedFileHeader.Length; i++)
        {
            var cellValue = row.Cell(i + 1).GetValue<string>().Trim();
            if (!cellValue.Equals(ExpectedFileHeader[i], StringComparison.OrdinalIgnoreCase)) return false;
        }

        return true;
    }

    private static bool IsDataObjectIFormFile(object data)
    {
        return data is IFormFile;
    }
}