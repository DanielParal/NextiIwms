using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexticz.Module.Vh.Application.Common.Helpers;
using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Module.Vh.Domain.BandRewards;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;
using Nexticz.Module.Vh.Domain.LoadingDevices;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.Partners;
using Nexticz.Module.Vh.Domain.ReportActivities;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option) : base(option)
    {
    }

    public DbSet<Center> Centers { get; set; } = null!;
    public DbSet<Depositor> Depositors { get; set; } = null!;
    public DbSet<DepositorsGroup> DepositorsGroups { get; set; } = null!;
    public DbSet<Worker> Workers { get; set; } = null!;
    public DbSet<ActivityCategory> ActivityCategories { get; set; } = null!;
    public DbSet<NonDispensingActivity> NonDispensingActivities { get; set; } = null!;
    public DbSet<SystemActivity> SystemActivities { get; set; } = null!;
    public DbSet<Assortment> Assortments { get; set; } = null!;
    public DbSet<BandReward> BandRewards { get; set; } = null!;
    public DbSet<Partner> Partners { get; set; } = null!;
    public DbSet<LoadingDevice> LoadingDevices { get; set; } = null!;
    public DbSet<LoadingActionsNda> LoadingActionsNdas { get; set; } = null!;
    public DbSet<LoadedActivity> LoadedActivities { get; set; } = null!;
    public DbSet<WorkerShift> WorkerShifts { get; set; } = null!;
    public DbSet<ShiftMasterChange> ShiftMasterChanges { get; set; } = null!;
    public DbSet<ReportActivity> ReportActivities { get; set; } = null!;
    public DbSet<ReportPerformanceEvaluation> ReportPerformanceEvaluations { get; set; } = null!;
    public DbSet<VhUser> VhUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.HasDefaultSchema(StringHelper.MigrationSchema);
    }
}