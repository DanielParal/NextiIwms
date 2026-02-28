using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Module.Vh.Domain.ReportActivities;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports.Mappers;

public static class ReportActivitiesMappers
{
    public static ReportActivityResponse MapToReportActivitiesResponse(this ReportActivity reportActivity)
    {
        return new ReportActivityResponse
        {
            Id = reportActivity.Id,
            WorkerShiftId = reportActivity.WorkerShiftId,
            Coefficient = reportActivity.Coefficient,
            Date = reportActivity.Date,
            Score = reportActivity.Score,
            ActivitiesCount = reportActivity.ActivitiesCount,
            ActivityCode = reportActivity.ActivityCode,
            ActivitySource = Enum.Parse<ActivitySource>(reportActivity.ActivitySource.ToString()),
            WorkerCenterCode = reportActivity.WorkerCenterCode,
            CenterCode = reportActivity.CenterCode,
            DurationTime = reportActivity.DurationTime,
            WorkerCode = reportActivity.WorkerCode,
            WorkerShiftEnd = reportActivity.WorkerShiftEnd,
            WorkerShiftStart = reportActivity.WorkerShiftStart,
            WorkerShiftPowerPercentage = reportActivity.WorkerShiftPowerPercentage,
            Note = reportActivity.Note
        };
    }
}