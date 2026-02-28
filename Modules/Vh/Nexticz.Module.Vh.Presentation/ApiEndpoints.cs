namespace Nexticz.Module.Vh.Presentation;

public static class ApiEndpoints
{
    private const string ApiBase = "/api/vh";

    public static class Groups
    {
        public const string Vh = nameof(Vh);
        public static readonly string[] All = [Vh];
    }

    public static class VhUsers
    {
        private const string Base = $"{ApiBase}/vhUsers";

        public const string GetVhUserById = $"{Base}/{{id}}";
        public const string GetVhUsers = $"{Base}";
        public const string CreateVhUser = $"{Base}";
        public const string UpdateVhUser = $"{Base}/{{id}}";
        public const string DeleteVhUser = $"{Base}/{{id}}";
    }
    public static class Centers
    {
        private const string Base = $"{ApiBase}/centers";

        public const string GetCenterById = $"{Base}/{{id}}";
        public const string GetCenters = $"{Base}";
        public const string CreateCenter = $"{Base}";
        public const string UpdateCenter = $"{Base}/{{id}}";
        public const string DeleteCenter = $"{Base}/{{id}}";
    }

    public static class Depositors
    {
        private const string Base = $"{ApiBase}/depositors";

        public const string GetDepositorById = $"{Base}/{{id}}";
        public const string GetDepositors = $"{Base}";
        public const string CreateDepositor = $"{Base}";
        public const string UpdateDepositor = $"{Base}/{{id}}";
        public const string DeleteDepositor = $"{Base}/{{id}}";
    }

    public static class DepositorsGroups
    {
        private const string Base = $"{ApiBase}/depositorsGroups";

        public const string GetDepositorsGroupById = $"{Base}/{{id}}";
        public const string GetDepositorsGroups = $"{Base}";
        public const string CreateDepositorsGroup = $"{Base}";
        public const string UpdateDepositorsGroup = $"{Base}/{{id}}";
        public const string DeleteDepositorsGroup = $"{Base}/{{id}}";
    }

    public static class Workers
    {
        private const string Base = $"{ApiBase}/workers";

        public const string GetWorkerByIdOrSlug = $"{Base}/{{idOrSlug}}";
        public const string GetWorkers = $"{Base}";
        public const string CreateWorker = $"{Base}";
        public const string UpdateWorker = $"{Base}/{{id}}";
        public const string DeleteWorker = $"{Base}/{{id}}";
    }

    public static class WorkerShifts
    {
        private const string Base = $"{ApiBase}/workerShifts";

        public const string GetWorkerShifts = $"{Base}";
        public const string AddWorkerShift = $"{Base}";
        public const string AddWorkerShiftActivity = $"{Base}/{{id}}/activities";
        public const string EndWorkerShift = $"{Base}/{{id}}/endings";
        public const string SetWorkerShiftApproval = $"{Base}/{{id}}/approvals";
    }

    public static class ShiftMasterChanges
    {
        private const string Base = $"{ApiBase}/shiftMasterChanges";

        public const string GetShiftMasterChanges = $"{Base}";
    }

    public static class ActivityCategories
    {
        private const string Base = $"{ApiBase}/activityCategories";

        public const string GetActivityCategoryById = $"{Base}/{{id}}";
        public const string GetActivityCategories = $"{Base}";
        public const string CreateActivityCategory = $"{Base}";
        public const string UpdateActivityCategory = $"{Base}/{{id}}";
        public const string DeleteActivityCategory = $"{Base}/{{id}}";
    }

    public static class NonDispensingActivities
    {
        private const string Base = $"{ApiBase}/nonDispensingActivities";

        public const string GetNonDispensingActivityByIdOrSlug = $"{Base}/{{idOrSlug}}";
        public const string GetNonDispensingActivities = $"{Base}";
        public const string CreateNonDispensingActivity = $"{Base}";
        public const string UpdateNonDispensingActivity = $"{Base}/{{id}}";
        public const string DeleteNonDispensingActivity = $"{Base}/{{id}}";
    }

    public static class SystemActivities
    {
        private const string Base = $"{ApiBase}/systemActivities";

        public const string GetSystemActivityById = $"{Base}/{{id}}";
        public const string GetSystemActivities = $"{Base}";
        public const string CreateSystemActivity = $"{Base}";
        public const string UpdateSystemActivity = $"{Base}/{{id}}";
        public const string DeleteSystemActivity = $"{Base}/{{id}}";
    }

    public static class Assortments
    {
        private const string Base = $"{ApiBase}/assortments";

        public const string GetAssortmentById = $"{Base}/{{id}}";
        public const string GetAssortments = $"{Base}";
        public const string CreateAssortment = $"{Base}";
        public const string UpdateAssortment = $"{Base}/{{id}}";
        public const string DeleteAssortment = $"{Base}/{{id}}";
    }

    public static class BandRewards
    {
        private const string Base = $"{ApiBase}/bandRewards";

        public const string GetBandRewardById = $"{Base}/{{id}}";
        public const string GetBandRewards = $"{Base}";
        public const string CreateBandReward = $"{Base}";
        public const string UpdateBandReward = $"{Base}/{{id}}";
        public const string DeleteBandReward = $"{Base}/{{id}}";
    }

    public static class Partners
    {
        private const string Base = $"{ApiBase}/partners";

        public const string GetPartnerdById = $"{Base}/{{id}}";
        public const string GetPartners = $"{Base}";
        public const string CreatePartner = $"{Base}";
        public const string UpdatePartner = $"{Base}/{{id}}";
        public const string DeletePartner = $"{Base}/{{id}}";
    }

    public static class LoadingDevices
    {
        private const string Base = $"{ApiBase}/loadingDevices";

        public const string RegisterLoadingDevice = $"{Base}/{{id}}/registrations";
        public const string GetLoadingDeviceByDeviceKey = $"{Base}/{{deviceKey}}";
        public const string GetLoadingDevices = $"{Base}";
        public const string CreateLoadingDevice = $"{Base}";
        public const string UpdateLoadingDevice = $"{Base}/{{id}}";
        public const string DeleteLoadingDevice = $"{Base}/{{id}}";
    }

    public static class LoadedActivities
    {
        private const string Base = $"{ApiBase}/loadedActivities";

        public const string GetLoadedActivityById = $"{Base}/{{id}}";
        public const string GetLoadedActivities = $"{Base}";
        public const string UpdateLoadedActivity = $"{Base}/{{id}}";
        public const string ImportLoadedActivities = $"{Base}";
    }

    public static class LoadingActionsNdas
    {
        private const string Base = $"{ApiBase}/loadingActionsNdas";

        public const string GetLoadingActionsNdaById = $"{Base}/{{id}}";
        public const string GetLoadingActionsNdas = $"{Base}";
        public const string CreateLoadingActionsNda = $"{Base}";
        public const string UpdateLoadingActionsNda = $"{Base}/{{id}}";
        public const string DeleteLoadingActionsNda = $"{Base}/{{id}}";
    }

    public static class SagDynamicsActivities
    {
        private const string Base = $"{ApiBase}/sagDynamicsActivities";

        public const string LoadDynamicsActivities = $"{Base}";
    }

    public static class Reports
    {
        private const string Base = $"{ApiBase}/reports";

        public const string GetReportActivities = $"{Base}/reportActivities";
        public const string GetReportWorkerShiftPerformance = $"{Base}/reportWorkerShiftPerformance";
        public const string GetReportActivitiesPerformance = $"{Base}/reportActivitiesPerformance";
        public const string GetReportPerformanceEvaluation = $"{Base}/reportPerformanceEvaluation";
        public const string GetDashboard = $"{Base}/dashboard";
        public const string GetDashboardPda = $"{Base}/dashboardPda";
    }
}