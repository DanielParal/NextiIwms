using ClosedXML.Excel;
using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivity;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;
using Nexticz.Module.Vh.Application.Workers.Common.Models;

namespace Nexticz.Module.Vh.Application.SagDynamicsActivities.Command.LoadSagDynamicsActivities;

public class LoadSagDynamicsActivitiesCommandHandler(IUnitOfWork unitOfWork, ISender mediatr)
    : IRequestHandler<LoadSagDynamicsActivitiesCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(LoadSagDynamicsActivitiesCommand command,
        CancellationToken cancellationToken)
    {
        var file = command.FormCollection.Files.GetFile("file");

        if (file is not { Length: > 0 })
            return Error.Validation("Empty file");

        using var stream = new MemoryStream();

        await file.CopyToAsync(stream, cancellationToken);

        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet is null)
            return Error.Validation("vh-api-sagService-emptyWorkSheet", "Empty worksheet");

        var workers =
            (await unitOfWork.WorkersRepository.GetWorkersAsync(new WorkersFilteringParams(), cancellationToken)).data
            .OfType<WorkerResponse>().ToList();

        var systemActivities =
            (await unitOfWork.SystemActivitiesRepository.GetSystemActivitiesAsync(new SystemActivitiesFilteringParams(),
                cancellationToken)).data.OfType<SystemActivityResponse>().Where(x => x.SystemType == "DYNAMICS")
            .ToList();

        var lastSagDynamicsLoadedActivity =
            await unitOfWork.LoadedActivitiesRepository.GetLastSagDynamicsLoadedActivityAsync(cancellationToken);

        List<LoadedActivity> addedLoadedActivities = [];

        var rowCount = worksheet.LastRowUsed()!.RowNumber();

        for (var row = 2; row <= rowCount; row++)
        {
            var activityStart = (DateTime)worksheet.Cell(row, 1).Value;
            var workerCode = worksheet.Cell(row, 2).Value;
            var activityCode = worksheet.Cell(row, 3).Value;

            if (activityStart <= lastSagDynamicsLoadedActivity?.Start) continue;

            var worker = GetWorkerCode(workers, workerCode.ToString());

            if (worker is null)
                return Error.Validation("vh-api-sagService-invalidWorkerCode",
                    $"Invalid worker code {workerCode.ToString()}");

            var activity = GetSagActivity(systemActivities, activityCode.ToString());

            if (activity is null)
                return Error.Validation("vh-api-sagService-invalidActivityCode",
                    $"Invalid activity code {activityCode.ToString()}");

            var parseSuccess = DateTime.TryParse(worksheet.Cell(row - 1, 1).Value.ToString(), out var dateValue);

            if (parseSuccess && activityStart == dateValue &&
                (string)workerCode == worksheet.Cell(row - 1, 2).Value.ToString())
            {
                activityStart = activityStart.AddSeconds(1);    
            }
            
            addedLoadedActivities.Add(new LoadedActivity(
                    DateTime.Now,
                    activityStart,
                    worker.CodeWms,
                    "309",
                    activityCode.ToString(),
                    null,
                    "PRUMCHEMIE_C",
                    "Nevýdejová",
                    ActivityType.PaidDynamics,
                    ActivitySource.Dynamics,
                    ActivityState.Copied,
                    false)
                {
                    ActivityCutOff = activity.CutOff,
                    Unit = activity.Unit,
                    Coefficient = activity.Coefficient,
                    ActivityName = activity.Name
                }
            );
        }

        foreach (var addedLoadedActivity in addedLoadedActivities)
            await mediatr.Send(new CreateLoadedActivityCommand { LoadedActivity = addedLoadedActivity },
                cancellationToken);

        return Result.Created;
    }

    private WorkerResponse? GetWorkerCode(List<WorkerResponse> workers, string workerIdDynamics)
    {
        return workers.FirstOrDefault(x => x.CodeSag == workerIdDynamics);
    }

    private SystemActivityResponse? GetSagActivity(List<SystemActivityResponse> systemActivities, string activityCode)
    {
        return systemActivities.FirstOrDefault(x => x.ActionCodeWms == activityCode);
    }
}