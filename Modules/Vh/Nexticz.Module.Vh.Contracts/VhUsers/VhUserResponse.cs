namespace Nexticz.Module.Vh.Contracts.VhUsers;

public class VhUserResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public string[] Centers { get; set; } = [];
    public VhRole[] Roles { get; set; } = [];
    public VhPermission[] VhPermissions { get; set; } = [];
}

public enum VhRole
{
    VhMember
}

public enum VhPermission
{
    VhMasterPreviewWindow,
    VhSettingsLoadedActionsNdas,
    VhReportsDashboard,
    VhReportsMasterChanges,
    VhReportsShiftPerformance,
    VhReportsPerformanceEvaluation,
    VhReportsReportActivities
}