using Nexticz.Lib.Shared.DataAccess.EntityFramework;

namespace Nexticz.Module.Vh.Application.Common.Interfaces;

public interface IUnitOfWork : IBaseUnitOfWork
{
    ICentersRepository CentersRepository { get; }
    IDepositorsRepository DepositorsRepository { get; }
    IDepositorsGroupsRepository DepositorsGroupsRepository { get; }
    IWorkersRepository WorkersRepository { get; }
    IActivityCategoriesRepository ActivityCategoriesRepository { get; }
    INonDispensingActivitiesRepository NonDispensingActivitiesRepository { get; }
    ISystemActivitiesRepository SystemActivitiesRepository { get; }
    IAssortmentRepository AssortmentRepository { get; }
    IBandRewardsRepository BandRewardsRepository { get; }
    IPartnersRepository PartnersRepository { get; }
    ILoadingDevicesRepository LoadingDevicesRepository { get; }
    ILoadingActionsNdasRepository LoadingActionsNdasRepository { get; }
    ILoadedActivitiesRepository LoadedActivitiesRepository { get; }
    IWorkerShiftsRepository WorkerShiftsRepository { get; }
    IShiftMasterChangesRepository ShiftMasterChangesRepository { get; }
    IReportActivitiesRepository ReportActivitiesRepository { get; }
    IReportPerformanceEvaluationRepository ReportPerformanceEvaluationRepository { get; }
    IVhUsersRepository VhUsersRepository { get; }
}