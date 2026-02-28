using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;
using Nexticz.Module.Vh.Application.Assortments.Common.Models;
using Nexticz.Module.Vh.Application.Centers.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.Depositors.Common.Models;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;
using Nexticz.Module.Vh.Application.Partners.Common.Models;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;

namespace Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents.MetadataCalculators;

public class MyStockHandlersCalculators(IUnitOfWork unitOfWork)
{
    private List<ActivityCategoryResponse> _activityCategories = [];
    private Guid _activityCategoryId = Guid.Empty;
    private string _activityCode = string.Empty;
    private string _assortmentCode = string.Empty;
    private List<AssortmentResponse> _assortments = [];
    private Guid _centerId = Guid.Empty;
    private List<CenterResponse> _centers = [];
    private Guid _depositorGroupId = Guid.Empty;
    private List<DepositorResponse> _depositors = [];
    private List<DepositorsGroupResponse> _depositorsGroups = [];
    private string _partnerCode = string.Empty;
    private List<PartnerResponse> _partners = [];
    private bool _pickingPlaceHigh;
    private List<SystemActivityResponse> _systemActivities = [];
    private SystemActivityResponse _systemActivity = null!;
    private string _systemType = string.Empty;

    public async Task InitializeAsync()
    {
        _centers = await GetCenters();
        _depositors = await GetDepositors();
        _depositorsGroups = await GetDepositorsGroups();
        _systemActivities = await GetSystemActivities();
        _activityCategories = await GetActivityCategories();
        _partners = await GetPartners();
        _assortments = await GetAssortments();
    }

    public void InitializeItemProperties(
        string activityCode,
        string depositorCode,
        string systemType,
        string depositorGroupCode,
        string assortmentCode,
        string partnerCode,
        bool pickingPlaceHigh
    )
    {
        _centerId = _depositors.First(x => x.Code == depositorCode).CenterId;
        _depositorGroupId = _depositorsGroups.First(x => x.Code == depositorGroupCode).Id;
        _activityCode = activityCode;
        _systemType = systemType;
        _assortmentCode = assortmentCode;
        _partnerCode = partnerCode;
        _pickingPlaceHigh = pickingPlaceHigh;
        _systemActivity = GetCorrectSystemActivity();
        _activityCategoryId = _systemActivity.ActivityCategoryId;
    }

    public string GetCenterCode()
    {
        var centerCode = _centers
            .First(x => x.Id == _centerId)
            .Code;

        return centerCode;
    }

    public string GetSystemActivityType()
    {
        var systemActivityType = _systemActivity.Type;

        return systemActivityType;
    }

    public ActivityType GetActivityType()
    {
        var activityType = (ActivityType)_activityCategories
            .First(x => x.Id == _activityCategoryId)
            .ActivityType;

        return activityType;
    }

    public TimeOnly? GetActivityCutOff()
    {
        var activityCutOff = _systemActivity.CutOff;

        return activityCutOff;
    }

    public string GetUnit()
    {
        var unit = _systemActivity.Unit;

        return unit;
    }

    public decimal GetCoefficient()
    {
        var coefficient = _systemActivity.Coefficient;

        var partnerExist = _partners.FirstOrDefault(x => x.Code == _partnerCode);

        if (partnerExist is not null)
            coefficient += _activityCode switch
            {
                "PRIJEM" => partnerExist.Receipt ? partnerExist.ReceiptCoefficient : 0,
                "VYCHYSTAVANI" => partnerExist.Dispatch ? partnerExist.DispatchCoefficient : 0,
                "BALENI" => partnerExist.Packaging ? partnerExist.PackagingCoefficient : 0,
                _ => 0
            };

        var assortmentExist = _assortments.FirstOrDefault(x => x.Code == _assortmentCode);

        if (assortmentExist is not null)
            coefficient += _partnerCode switch
            {
                "NASKLADNENI" => assortmentExist.Receipt ? assortmentExist.ReceiptCoefficient : 0,
                "VYCHYSTAVANI" => assortmentExist.Dispatch ? assortmentExist.DispatchCoefficient : 0,
                "BALENI" => assortmentExist.Packaging ? assortmentExist.PackagingCoefficient : 0,
                _ => 0
            };

        return coefficient;
    }

    public string GetActivityName()
    {
        var activityName = _systemActivity.Name;

        return activityName;
    }

    private SystemActivityResponse GetCorrectSystemActivity()
    {
        return IsSpecialPickingHighAndStdActivity()
            ? _systemActivities.First(x =>
                x.ActionCodeWms == _activityCode &&
                x.SystemType == _systemType &&
                x.DepositorGroupId == _depositorGroupId &&
                x.Name == "Vychystávání Vysoké")
            : _systemActivities.First(x =>
                x.ActionCodeWms == _activityCode &&
                x.SystemType == _systemType &&
                x.DepositorGroupId == _depositorGroupId);
    }

    private bool IsSpecialPickingHighAndStdActivity()
    {
        return _pickingPlaceHigh && _activityCode == "VYCHYSTAVANI" && _systemType == "STD";
    }

    private async Task<List<CenterResponse>> GetCenters()
    {
        var filters = new CentersFilteringParams { Skip = "0" };

        return (await unitOfWork.CentersRepository
                .GetCentersResponseAsync(filters, CancellationToken.None))
            .data.OfType<CenterResponse>()
            .ToList();
    }

    private async Task<List<DepositorResponse>> GetDepositors()
    {
        var filters = new DepositorsFilteringParams();

        return (await unitOfWork.DepositorsRepository
                .GetDepositorsAsync(filters, CancellationToken.None))
            .data.OfType<DepositorResponse>()
            .ToList();
    }

    private async Task<List<DepositorsGroupResponse>> GetDepositorsGroups()
    {
        var filters = new DepositorsGroupsFilteringParams();

        return (await unitOfWork.DepositorsGroupsRepository
                .GetDepositorsGroupsAsync(filters, CancellationToken.None))
            .data.OfType<DepositorsGroupResponse>()
            .ToList();
    }

    private async Task<List<SystemActivityResponse>> GetSystemActivities()
    {
        var filters = new SystemActivitiesFilteringParams();

        return (await unitOfWork.SystemActivitiesRepository
                .GetSystemActivitiesAsync(filters, CancellationToken.None))
            .data.OfType<SystemActivityResponse>()
            .ToList();
    }

    private async Task<List<ActivityCategoryResponse>> GetActivityCategories()
    {
        var filters = new ActivityCategoriesFilteringParams();

        return (await unitOfWork.ActivityCategoriesRepository
                .GetActivityCategoriesAsync(filters, CancellationToken.None))
            .data.OfType<ActivityCategoryResponse>()
            .ToList();
    }

    private async Task<List<PartnerResponse>> GetPartners()
    {
        var filters = new PartnersFilteringParams();

        return (await unitOfWork.PartnersRepository
                .GetPartnersAsync(filters, CancellationToken.None))
            .data.OfType<PartnerResponse>()
            .ToList();
    }

    private async Task<List<AssortmentResponse>> GetAssortments()
    {
        var filters = new AssortmentsFilteringParams();

        return (await unitOfWork.AssortmentRepository
                .GetAssortmentsAsync(filters, CancellationToken.None))
            .data.OfType<AssortmentResponse>()
            .ToList();
    }
}