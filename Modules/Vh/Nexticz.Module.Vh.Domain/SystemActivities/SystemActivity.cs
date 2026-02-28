using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Domain.DepositorsGroups;

namespace Nexticz.Module.Vh.Domain.SystemActivities;

public class SystemActivity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string ActionCodeWms { get; set; }
    public required string SystemType { get; set; }
    public required Guid DepositorGroupId { get; set; }
    public DepositorsGroup? DepositorsGroup { get; set; }
    public required Guid ActivityCategoryId { get; set; }
    public ActivityCategory? ActivityCategory { get; set; }
    public required string Type { get; set; }
    public required string WhatToMeasure { get; set; }
    public required string Unit { get; set; }
    public required decimal Coefficient { get; set; }
    public TimeOnly? CutOff { get; set; }
}