using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Infrastructure.ActivityCategories.Persistence;
using Nexticz.Module.Vh.Infrastructure.Assortments.Persistence;
using Nexticz.Module.Vh.Infrastructure.BandRewards.Persistence;
using Nexticz.Module.Vh.Infrastructure.Centers.Persistence;
using Nexticz.Module.Vh.Infrastructure.Depositors.Persistence;
using Nexticz.Module.Vh.Infrastructure.DepositorsGroups.Persistence;
using Nexticz.Module.Vh.Infrastructure.LoadedActivities.Persistence;
using Nexticz.Module.Vh.Infrastructure.LoadingActionsNdas.Persistence;
using Nexticz.Module.Vh.Infrastructure.LoadingDevices.Persistence;
using Nexticz.Module.Vh.Infrastructure.NonDispensingActivities.Persistence;
using Nexticz.Module.Vh.Infrastructure.Partners.Persistence;
using Nexticz.Module.Vh.Infrastructure.ReportActivities.Persistence;
using Nexticz.Module.Vh.Infrastructure.ReportPerformanceEvaluations.Persistence;
using Nexticz.Module.Vh.Infrastructure.ShiftMasterChanges.Persistence;
using Nexticz.Module.Vh.Infrastructure.SystemActivities.Persistence;
using Nexticz.Module.Vh.Infrastructure.VhUsers.Persistence;
using Nexticz.Module.Vh.Infrastructure.Workers.Persistence;
using Nexticz.Module.Vh.Infrastructure.WorkerShifts.Persistence;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    public ICentersRepository CentersRepository => new CentersRepository(context);
    public IDepositorsRepository DepositorsRepository => new DepositorsRepository(context);
    public IDepositorsGroupsRepository DepositorsGroupsRepository => new DepositorsGroupsRepository(context);
    public IWorkersRepository WorkersRepository => new WorkersRepository(context);
    public IActivityCategoriesRepository ActivityCategoriesRepository => new ActivityCategoriesRepository(context);

    public INonDispensingActivitiesRepository NonDispensingActivitiesRepository =>
        new NonDispensingActivitiesRepository(context);

    public ISystemActivitiesRepository SystemActivitiesRepository => new SystemActivitiesRepository(context);
    public IAssortmentRepository AssortmentRepository => new AssortmentsRepository(context);
    public IBandRewardsRepository BandRewardsRepository => new BandRewardsRepository(context);
    public IPartnersRepository PartnersRepository => new PartnersRepository(context);
    public ILoadingDevicesRepository LoadingDevicesRepository => new LoadingDevicesRepository(context);
    public ILoadingActionsNdasRepository LoadingActionsNdasRepository => new LoadingActionsNdasRepository(context);
    public ILoadedActivitiesRepository LoadedActivitiesRepository => new LoadedActivitiesRepository(context);
    public IWorkerShiftsRepository WorkerShiftsRepository => new WorkerShiftsRepository(context);
    public IShiftMasterChangesRepository ShiftMasterChangesRepository => new ShiftMasterChangesRepository(context);
    public IReportActivitiesRepository ReportActivitiesRepository => new ReportActivitiesRepository(context);

    public IReportPerformanceEvaluationRepository ReportPerformanceEvaluationRepository =>
        new ReportPerformanceEvaluationRepository(context);

    public IVhUsersRepository VhUsersRepository => new VhUsersRepository(context);

    public async Task<bool> CompleteAsync(CancellationToken token = default)
    {
        var count = await context.SaveChangesAsync(token);
        return count > 0;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = default)
    {
        return await context.Database.BeginTransactionAsync(token);
    }

    public void Add(object item)
    {
        context.Add(item);
    }

    public void Update(object item)
    {
        context.Update(item);
    }

    public void Remove(object item)
    {
        context.Remove(item);
    }

    public void AddRange(IEnumerable<object> items)
    {
        context.AddRange(items);
    }

    public void UpdateRange(IEnumerable<object> items)
    {
        context.UpdateRange(items);
    }

    public void RemoveRange(IEnumerable<object> items)
    {
        context.RemoveRange(items);
    }

    public DbContext GetContext()
    {
        return context;
    }
}