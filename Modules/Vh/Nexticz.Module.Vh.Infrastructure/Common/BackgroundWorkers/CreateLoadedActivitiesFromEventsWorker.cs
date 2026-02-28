using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;
using Nexticz.Module.Vh.Application.ActivityCategories.Queries.GetActivityCategoryById;
using Nexticz.Module.Vh.Application.Assortments.Common.Models;
using Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortments;
using Nexticz.Module.Vh.Application.Centers.Common.Models;
using Nexticz.Module.Vh.Application.Centers.Queries.ListCenters;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.Depositors.Common.Models;
using Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositors;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;
using Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroups;
using Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivity;
using Nexticz.Module.Vh.Application.LoadedActivities.Common.Models;
using Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastIwmsLoadedActivity;
using Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastMyStockLoadedActivity;
using Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLoadedActivities;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Common.Models;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdas;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivities;
using Nexticz.Module.Vh.Application.Partners.Common.Models;
using Nexticz.Module.Vh.Application.Partners.Queries.GetPartners;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;
using Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivities;
using Nexticz.Module.Vh.Application.WorkerShifts.Commands.EndWorkerShift;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;
using Nexticz.Module.Vh.Application.WorkerShifts.Notifications.WorkerShiftApprovalChanged;
using Nexticz.Module.Vh.Application.WorkerShifts.Queries.GetWorkerShifts;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Infrastructure.Common.BackgroundWorkers.Mappers;
using Nexticz.Lib.Shared.BackgroundServices;
using Nexticz.Module.Vh.Infrastructure.Common.Persistence.VYKHOD;
using ActivitySource = Nexticz.Module.Vh.Domain.WorkerShifts.ActivitySource;
using ActivityType = Nexticz.Module.Vh.Domain.WorkerShifts.ActivityType;

namespace Nexticz.Module.Vh.Infrastructure.Common.BackgroundWorkers;

public class CreateLoadedActivitiesFromEventsWorker(
    IServiceScopeFactory serviceScopeFactory,
    IFeatureManager featureManager,
    ILogger<CreateLoadedActivitiesFromEventsWorker> logger)
    : BaseBackgroundService(featureManager, logger), ICreateLoadedActivitiesFromEventsWorker
{
    private const string WorkerVhCreateLoadedActivitiesFromEventsWorkerEnabled = nameof(WorkerVhCreateLoadedActivitiesFromEventsWorkerEnabled);
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(120));
    private readonly IFeatureManager _featureManager = featureManager;

    public async Task ExecuteWorkAsync(CancellationToken stoppingToken)
    {
        var scope = serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var env = services.GetRequiredService<IWebHostEnvironment>();

        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _featureManager.IsEnabledAsync(nameof(WorkerVhCreateLoadedActivitiesFromEventsWorkerEnabled)))
                    continue;
                
                if (env.IsProduction())
                {
                    logger.LogInformation("Worker is running in production mode");
                    await CreateLoadedActivitiesFromIwmsEvents();
                    await CreateLoadingActivitiesFromMyStockEvents();
                    await CreateWorkerShiftsFromLoadedActivities();
                }

                if (env.IsStaging())
                {
                    logger.LogInformation("Worker is running in staging mode");
                    await CreateLoadedActivitiesFromIwmsEvents();
                    await CreateLoadingActivitiesFromMyStockEvents();
                    await CreateWorkerShiftsFromLoadedActivities();
                }

                if (env.IsDevelopment())
                {
                    logger.LogInformation("Worker is running in development mode");
                    // await CreateLoadingActivitiesFromMyStockEvents();
                    await CreateLoadedActivitiesFromIwmsEvents();
                    await CreateWorkerShiftsFromLoadedActivities();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "CRON ERROR");
            }
        }
            
    }

    protected override async Task ExecuteWorkerAsync(CancellationToken stoppingToken)
    {
        await ExecuteWorkAsync(stoppingToken);
    }

    private async Task CreateLoadedActivitiesFromIwmsEvents()
    {
        var scope = serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var mediatr = services.GetRequiredService<ISender>();

        string? filter = null;
        List<LoadedActivity> addedLoadedActivities = [];

        var queryNdaActivities = new GetNonDispensingActivitiesQuery
            { FilteringParams = new NonDispensingActivitiesFilteringParams() };
        var nonDispensingActivities =
            (await mediatr.Send(queryNdaActivities)).Value.data.OfType<NonDispensingActivity>().ToList();

        var queryLoadedActivity = new GetLastIwmsLoadedActivityQuery();
        var lastIwmsLoadedActivity = (await mediatr.Send(queryLoadedActivity)).Value;

        if (lastIwmsLoadedActivity is not null)
            filter =
                $"[\"{nameof(lastIwmsLoadedActivity.Created).ToLower()}\", \">\", \"{lastIwmsLoadedActivity.Created.ToUniversalTime():O}\"]";

        var filteringParams = new LoadingActionsNdasFilteringParams
        {
            Filter = filter
        };

        var queryIwmsEvents = new GetLoadingActionsNdasQuery { FilteringParams = filteringParams };
        var iwmsEvents = (await mediatr.Send(queryIwmsEvents)).Value.data.OfType<LoadingActionsNdaResponse>().ToList();

        foreach (var o in iwmsEvents)
        {
            var detailedInfo = GetDetailInfoIwms(o.NonDispensingActivitySlug, nonDispensingActivities);

            if (detailedInfo.centerCode is null || detailedInfo.activityType is null)
            {
                logger.LogError("Import iwmsItem ERROR");
                continue;
            }

            addedLoadedActivities.Add(new LoadedActivity(
                    DateTime.Now,
                    o.Created.ToLocalTime(),
                    o.WorkerSlug,
                    detailedInfo.centerCode,
                    o.NonDispensingActivitySlug,
                    null,
                    null,
                    "Nevýdejová",
                    (ActivityType)detailedInfo.activityType,
                    ActivitySource.Iwms,
                    ActivityState.Copied,
                    false)
                {
                    ActivityCutOff = detailedInfo.activityCutOff,
                    Note = o.Note,
                    LoadingDeviceId = o.LoadingDeviceId,
                    Unit = detailedInfo.unit,
                    Coefficient = detailedInfo.coefficient,
                    ActivityName = detailedInfo.activityName
                }
            );
        }

        foreach (var addedLoadedActivity in addedLoadedActivities)
        {
            var command = new CreateLoadedActivityCommand { LoadedActivity = addedLoadedActivity };
            await mediatr.Send(command);
        }
    }

    private (string? centerCode, ActivityType? activityType, string? unit, decimal coefficient, string activityName,
        TimeOnly? activityCutOff)
        GetDetailInfoIwms(
            string activityCode,
            List<NonDispensingActivity> nonDispensingActivities)
    {
        var centerCode = nonDispensingActivities.FirstOrDefault(x => x.ActivityIdentifier == activityCode)?.Center
            ?.Code;
        var activityType =
            nonDispensingActivities.FirstOrDefault(x => x.ActivityIdentifier == activityCode)?.ActivityCategory
                ?.ActivityType;
        var unit = nonDispensingActivities.FirstOrDefault(x => x.ActivityIdentifier == activityCode)?.Unit;
        var coefficient = nonDispensingActivities.FirstOrDefault(x => x.ActivityIdentifier == activityCode)
            ?.Coefficient ?? 0;

        var activityName = nonDispensingActivities.FirstOrDefault(x => x.ActivityIdentifier == activityCode)?.Name ??
                           "withoutName";

        var activityCutOff =
            nonDispensingActivities.FirstOrDefault(x => x.ActivityIdentifier == activityCode)?.CutOff ??
            null;

        return (centerCode, activityType, unit, coefficient, activityName, activityCutOff);
    }

    private async Task CreateLoadingActivitiesFromMyStockEvents()
    {
        var scope = serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var mediatr = services.GetRequiredService<ISender>();
        var vykhodDbContext = services.GetRequiredService<VykhodContext>();

        var filter = new DateTime(2024, 1, 1);
        List<LoadedActivity> addedLoadedActivities = [];

        var systemActivitiesQuery = new GetSystemActivitiesQuery
            { FilteringParams = new SystemActivitiesFilteringParams() };
        var systemActivities =
            (await mediatr.Send(systemActivitiesQuery)).Value.data.OfType<SystemActivityResponse>().ToList();

        var activityCategoryQuery = new GetActivityCategoriesQuery
            { FilteringParams = new ActivityCategoriesFilteringParams() };
        var activityCategories = (await mediatr.Send(activityCategoryQuery)).Value.data
            .OfType<ActivityCategoryResponse>().ToList();

        var depositorsQuery = new GetDepositorsQuery { FilteringParams = new DepositorsFilteringParams() };
        var depositors = (await mediatr.Send(depositorsQuery)).Value.data.OfType<DepositorResponse>().ToList();

        var centerQuery = new ListCentersQuery { FilteringParams = new CentersFilteringParams { Skip = "0" } };
        var centers = (await mediatr.Send(centerQuery)).Value.data.OfType<CenterResponse>().ToList();

        var partnerQuery = new GetPartnersQuery { FilteringParams = new PartnersFilteringParams() };
        var partners = (await mediatr.Send(partnerQuery)).Value.data.OfType<PartnerResponse>().ToList();

        var assortmentQuery = new GetAssortmentsQuery { FilteringParams = new AssortmentsFilteringParams() };
        var assortments = (await mediatr.Send(assortmentQuery)).Value.data.OfType<AssortmentResponse>().ToList();

        var depositorsGroupsQuery = new GetDepositorsGroupsQuery
            { FilteringParams = new DepositorsGroupsFilteringParams() };
        var depositorsGroups = (await mediatr.Send(depositorsGroupsQuery)).Value.data.OfType<DepositorsGroupResponse>()
            .ToList();

        var loadedActivityQuery = new GetLastMyStockLoadedActivityQuery();
        var lastMyStockLoadedActivity = (await mediatr.Send(loadedActivityQuery)).Value;

        if (lastMyStockLoadedActivity is not null)
        {
            filter = lastMyStockLoadedActivity.Created;
            filter = filter.AddMinutes(-15);
        }

        var myStockEvents = await vykhodDbContext.LgsVykonHodnocenis
            .Where(x => x.Datum > filter)
            .ToListAsync();

        var loadedActivitiesQuery = new GetLoadedActivitiesQuery
        {
            FilteringParams = new LoadedActivitiesFilteringParams
            {
                Filter = $"[\"{nameof(lastMyStockLoadedActivity.Created).ToLower()}\", \">\", \"{filter:O}\"]"
            }
        };
        var loadedActivities = (await mediatr.Send(loadedActivitiesQuery)).Value.data.OfType<LoadedActivity>().ToList();

        foreach (var myStockEvent in myStockEvents)
        {
            if (IsMyStockEventCopied(myStockEvent, loadedActivities))
                continue;

            var detailedInfo = GetDetailInfoMyStock(myStockEvent.AkceKod, myStockEvent.UkladatelKod, centers,
                depositors, systemActivities,
                activityCategories, depositorsGroups, myStockEvent.LicenceKod, myStockEvent.UkladatelSkupKod, partners,
                assortments,
                myStockEvent.SortKod, myStockEvent.PartnerKod, myStockEvent.UmistVychL == 0);

            if (detailedInfo.centerCode is null || detailedInfo.activityType is null ||
                detailedInfo.activityType == ActivityType.Unknown || detailedInfo.activityCutOff is null)
            {
                logger.LogError("Import myStoctItem ERROR {@MyStockEvent}", myStockEvent);
                continue;
            }

            myStockEvent.UkladatelSkupKod = myStockEvent.UkladatelSkupKod == "ECOLAB_OST_C"
                ? "ECOLAB_C"
                : myStockEvent.UkladatelSkupKod;

            addedLoadedActivities.Add(new LoadedActivity(
                    DateTime.Now,
                    myStockEvent.Datum,
                    myStockEvent.OsobaKod,
                    detailedInfo.centerCode,
                    myStockEvent.AkceKod,
                    myStockEvent.UkladatelKod,
                    myStockEvent.UkladatelSkupKod,
                    detailedInfo.activitySystemType,
                    (ActivityType)detailedInfo.activityType,
                    ActivitySource.MyStock,
                    ActivityState.Copied,
                    myStockEvent.UmistVychL == 1)
                {
                    ActivityCutOff = detailedInfo.activityCutOff,
                    Unit = detailedInfo.unit,
                    Coefficient = detailedInfo.coefficient,
                    Idd = myStockEvent.Idd,
                    Idi = myStockEvent.Idi,
                    Idp = myStockEvent.Idp,
                    Idt = myStockEvent.Idt,
                    ActivityName = detailedInfo.activityName,
                    PDoklad = myStockEvent.PDoklad,
                    SortKod = myStockEvent.SortKod,
                    LicenceKod = myStockEvent.LicenceKod
                }
            );
        }

        foreach (var addedLoadedActivity in addedLoadedActivities)
        {
            var command = new CreateLoadedActivityCommand { LoadedActivity = addedLoadedActivity };
            await mediatr.Send(command);
        }
    }

    private bool IsMyStockEventCopied(LgsVykonHodnoceni myStockEvent, List<LoadedActivity> loadedActivities)
    {
        var loadedActivity = loadedActivities.FirstOrDefault(x =>
            x.Idd == myStockEvent.Idd && x.Idi == myStockEvent.Idi && x.Idp == myStockEvent.Idp &&
            x.Idt == myStockEvent.Idt);

        return loadedActivity is not null;
    }

    private (string? centerCode, ActivityType? activityType, TimeOnly? activityCutOff, string? unit, decimal coefficient
        , string activitySystemType, string activityName
        )
        GetDetailInfoMyStock(string activityCode, string depositor, List<CenterResponse> centers,
            List<DepositorResponse> depositors, List<SystemActivityResponse> activities,
            List<ActivityCategoryResponse> activityCategories, List<DepositorsGroupResponse> depositorsGroups,
            string systemCode, string depositorGroup, List<PartnerResponse> partners,
            List<AssortmentResponse> assortments, string sortCode, string partnerCode, bool pickingPlaceHight)
    {
        var centerId = depositors.FirstOrDefault(x => x.Code == depositor)?.CenterId;

        var depositorGroupId = depositorsGroups.FirstOrDefault(x => x.Code == depositorGroup)?.Id;

        var activityCategoryId = activities.FirstOrDefault(x =>
            x.ActionCodeWms == activityCode &&
            x.SystemType == systemCode &&
            x.DepositorGroupId == depositorGroupId)?.ActivityCategoryId;

        var centerCode = centers.FirstOrDefault(x => x.Id == centerId)?.Code;
        var activityType = activityCategories.FirstOrDefault(x => x.Id == activityCategoryId)?.ActivityType;
        var activityCutOff = activities.FirstOrDefault(x =>
            x.ActionCodeWms == activityCode &&
            x.SystemType == systemCode &&
            x.DepositorGroupId == depositorGroupId)?.CutOff;

        var parseResult = Enum.TryParse(activityType.ToString(), out ActivityType activityParsedType);

        if (!parseResult)
            activityParsedType = ActivityType.Unknown;

        var unit = activities.FirstOrDefault(x =>
            x.ActionCodeWms == activityCode &&
            x.SystemType == systemCode &&
            x.DepositorGroupId == depositorGroupId)?.Unit;
        var coefficient = activities.FirstOrDefault(x =>
                x.ActionCodeWms == activityCode &&
                x.SystemType == systemCode &&
                x.DepositorGroupId == depositorGroupId)
            ?.Coefficient ?? 0;

        if (pickingPlaceHight && activityCode == "VYCHYSTAVANI" && systemCode == "STD")
            coefficient = activities.FirstOrDefault(x =>
                    x.ActionCodeWms == activityCode &&
                    x.SystemType == systemCode &&
                    x.DepositorGroupId == depositorGroupId &&
                    x.Name == "Vychystávání Vysoké")
                ?.Coefficient ?? 0;

        var activitySystemType = activities.FirstOrDefault(x =>
            x.ActionCodeWms == activityCode &&
            x.SystemType == systemCode &&
            x.DepositorGroupId == depositorGroupId)?.Type;

        var activityName = activities.FirstOrDefault(x =>
                x.ActionCodeWms == activityCode &&
                x.SystemType == systemCode &&
                x.DepositorGroupId == depositorGroupId)
            ?.Name ?? "withoutName";

        if (pickingPlaceHight && activityCode == "VYCHYSTAVANI" && systemCode == "STD")
            activityName = activities.FirstOrDefault(x =>
                    x.ActionCodeWms == activityCode &&
                    x.SystemType == systemCode &&
                    x.DepositorGroupId == depositorGroupId &&
                    x.Name == "Vychystávání Vysoké")
                ?.Name ?? "withoutName";

        var partnerExist = partners.FirstOrDefault(x => x.Code == partnerCode);

        if (partnerExist is not null)
            switch (activityCode)
            {
                case "PRIJEM":
                {
                    if (partnerExist.Receipt)
                        coefficient += partnerExist.ReceiptCoefficient;
                    break;
                }
                case "VYCHYSTAVANI":
                {
                    if (partnerExist.Dispatch)
                        coefficient += partnerExist.DispatchCoefficient;
                    break;
                }
                case "BALENI":
                {
                    if (partnerExist.Packaging)
                        coefficient += partnerExist.PackagingCoefficient;
                    break;
                }
            }

        var assortmentExist = assortments.FirstOrDefault(x => x.Code == sortCode);

        if (assortmentExist is not null)
            switch (partnerCode)
            {
                case "NASKLADNENI":
                {
                    if (assortmentExist.Receipt)
                        coefficient += assortmentExist.ReceiptCoefficient;
                    break;
                }
                case "VYCHYSTAVANI":
                {
                    if (assortmentExist.Dispatch)
                        coefficient += assortmentExist.DispatchCoefficient;
                    break;
                }
                case "BALENI":
                {
                    if (assortmentExist.Packaging)
                        coefficient += assortmentExist.PackagingCoefficient;
                    break;
                }
            }

        return (centerCode, activityParsedType, activityCutOff, unit, coefficient, activitySystemType!, activityName);
    }

    private async Task CreateWorkerShiftsFromLoadedActivities()
    {
        var scope = serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var mediatr = services.GetRequiredService<IMediator>();
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();

        var filter = $"[\"{nameof(LoadedActivity.ActivityState).ToLower()}\",\"=\",\"{ActivityState.Copied}\"]";
        var copiedLoadedActivitiesQuery = new GetLoadedActivitiesQuery
            { FilteringParams = new LoadedActivitiesFilteringParams { Filter = filter } };
        var copiedLoadedActivities = (await mediatr.Send(copiedLoadedActivitiesQuery)).Value.data
            .OfType<LoadedActivity>().OrderBy(x => x.Start).ToList();

        foreach (var copiedLoadedActivity in copiedLoadedActivities)
        {
            var workerShifts = await GetWorkerShiftForCopiedActivity(copiedLoadedActivity, mediatr);

            if (IsActivityTypeWorkerShiftEnd(copiedLoadedActivity))
                await EndWorkerShift(workerShifts, copiedLoadedActivity, unitOfWork, mediatr);
            else
                await AddToWorkerShift(workerShifts, copiedLoadedActivity, unitOfWork, mediatr);
        }

        await CheckNotEndedWorkerShiftsActivitiesCutOffs(unitOfWork);
    }

    private async Task CheckNotEndedWorkerShiftsActivitiesCutOffs(IUnitOfWork unitOfWork)
    {
        var workerShifts = await unitOfWork.WorkerShiftsRepository.GetWorkerShiftsWithAnyNotEndedActivityAsync();

        foreach (var workerShift in workerShifts) workerShift.CheckLastNotEndedActivityCutOff();

        await unitOfWork.CompleteAsync();
    }

    private async Task EndWorkerShift(List<WorkerShift> workerShifts, LoadedActivity copiedLoadedActivity,
        IUnitOfWork unitOfWork, ISender mediatr)
    {
        var workerShift = FindNotEndedWorkerShiftForActivity(workerShifts, copiedLoadedActivity);

        if (workerShift is not null)
        {
            var endWorkerShiftCommand = new EndWorkerShiftCommand(workerShift.Id, copiedLoadedActivity.Start);

            var result = await mediatr.Send(endWorkerShiftCommand);
        }

        copiedLoadedActivity.ActivityState = ActivityState.Processed;

        await unitOfWork.CompleteAsync();
    }

    private async Task AddToWorkerShift(List<WorkerShift> workerShifts,
        LoadedActivity copiedLoadedActivity, IUnitOfWork unitOfWork, IMediator mediator)
    {
        if (IsFirstWorkerShift(workerShifts) || IsActivityAfterLastEndedWorkerShift(workerShifts, copiedLoadedActivity))
        {
            await CreteNewWorkerShiftWithActivity(copiedLoadedActivity, unitOfWork);
            return;
        }

        var workerShift = FindWorkerShiftForActivity(workerShifts, copiedLoadedActivity);

        if (workerShift?.Approved is true)
        {
            workerShift.SetApproval(false);
            var notification =
                new WorkerShiftApprovalChangedNotification(workerShift.Id, false);

            await mediator.Publish(notification);
        }

        if (workerShift is not null)
        {
            await AddActivityToExistingWorkerShift(workerShift, copiedLoadedActivity, unitOfWork);
            return;
        }

        copiedLoadedActivity.ActivityState = ActivityState.ProcessedError;
        await unitOfWork.CompleteAsync();
    }

    private WorkerShift? FindWorkerShiftForActivity(List<WorkerShift> workerShifts, LoadedActivity copiedLoadedActivity)
    {
        var endedWorkerShift = FindEndedWorkerShiftForActivity(workerShifts, copiedLoadedActivity);

        var notEndedWorkerShift = FindNotEndedWorkerShiftForActivity(workerShifts, copiedLoadedActivity);

        if (endedWorkerShift is not null) return endedWorkerShift;

        return notEndedWorkerShift ?? null;
    }

    private async Task AddActivityToExistingWorkerShift(WorkerShift workerShift, LoadedActivity copiedLoadedActivity,
        IUnitOfWork unitOfWork)
    {
        workerShift.AddActivity(
            copiedLoadedActivity.MapToWorkerShiftActivity());
        copiedLoadedActivity.ActivityState = ActivityState.Processed;
        await unitOfWork.CompleteAsync();
    }

    private async Task CreteNewWorkerShiftWithActivity(LoadedActivity copiedLoadedActivity, IUnitOfWork unitOfWork)
    {
        var worker =
            await unitOfWork.WorkersRepository.GetWorkerResponseBySlugAsync(copiedLoadedActivity.WorkerCode, default);

        if (worker is null)
        {
            logger.LogError("Import copiedLoadedActivity ERROR, worker not exist {@CopiedLoadedActivity}",
                copiedLoadedActivity);
            copiedLoadedActivity.ActivityState = ActivityState.ProcessedError;
            await unitOfWork.CompleteAsync();
            return;
        }

        var center = await unitOfWork.CentersRepository.GetCenterByIdAsync(worker.CenterId, default);

        if (center is null)
        {
            logger.LogError("Import copiedLoadedActivity ERROR, center not exist {@CopiedLoadedActivity}",
                copiedLoadedActivity);
            copiedLoadedActivity.ActivityState = ActivityState.ProcessedError;
            await unitOfWork.CompleteAsync();
            return;
        }

        var nonDispensingActivity =
            await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivityResponseBySlugAsync(
                worker.ActivityAfterCutOffCode, default);

        var activityName = nonDispensingActivity is not null
            ? nonDispensingActivity.Name
            : "";

        unitOfWork.Add(
            new WorkerShift(center.Code, copiedLoadedActivity.MapToWorkerShiftActivity())
            {
                ActivityAfterCutOffCode = worker.ActivityAfterCutOffCode,
                ActivityAfterCutOffName = activityName
            });
        copiedLoadedActivity.ActivityState = ActivityState.Processed;
        await unitOfWork.CompleteAsync();
    }

    private bool IsActivityTypeWorkerShiftEnd(LoadedActivity copiedLoadedActivity)
    {
        return copiedLoadedActivity.ActivityType == ActivityType.WorkerShiftEnd;
    }

    private bool IsFirstWorkerShift(List<WorkerShift> workerShifts)
    {
        return workerShifts.Count == 0;
    }

    private WorkerShift? FindEndedWorkerShiftForActivity(List<WorkerShift> workerShifts,
        LoadedActivity copiedLoadedActivity)
    {
        return workerShifts.Where(x => x.End > copiedLoadedActivity.Start).OrderBy(x => x.Start).FirstOrDefault();
    }

    private WorkerShift? FindNotEndedWorkerShiftForActivity(List<WorkerShift> workerShifts,
        LoadedActivity copiedLoadedActivity)
    {
        return workerShifts.Where(x => x.End == null).MaxBy(x => x.Start);
    }

    private bool IsActivityAfterLastEndedWorkerShift(List<WorkerShift> workerShifts,
        LoadedActivity copiedLoadedActivity)
    {
        var workerShiftWithoutEnd = workerShifts.Where(x => x.End == null).MaxBy(x => x.Start);

        var lastEndedWorkerShift = workerShifts.Where(x => x.End != null).MaxBy(x => x.End);

        if (workerShiftWithoutEnd is not null && workerShiftWithoutEnd.Start > lastEndedWorkerShift?.End)
            return false;

        return lastEndedWorkerShift is not null && copiedLoadedActivity.Start > lastEndedWorkerShift.End;
    }

    private async Task<List<WorkerShift>> GetWorkerShiftForCopiedActivity(LoadedActivity copiedLoadedActivity,
        ISender mediatr)
    {
        var filterWorkerShifts =
            $"[[\"{nameof(WorkerShift.WorkerCode)}\",\"=\",\"{copiedLoadedActivity.WorkerCode}\"], \"and\", [[\"{nameof(WorkerShift.End)}\",\"=\",null],\"or\",[\"{nameof(WorkerShift.End)}\",\">\",\"{copiedLoadedActivity.Start:O}\"]]]";
        var workerShiftsQuery = new GetWorkerShiftsQuery
            { FilteringParams = new WorkerShiftsFilteringParams { Filter = filterWorkerShifts } };
        return (await mediatr.Send(workerShiftsQuery)).Value.data.OfType<WorkerShift>().ToList();
    }
}