namespace Nexticz.Module.Vh.Contracts.SystemActivities;

public class UpdateSystemActivityRequest
{
    public required string Name { get; set; }
    public required string ActionCodeWms { get; set; }
    public required string SystemType { get; set; }
    public required Guid DepositorGroupId { get; set; }
    public required Guid ActivityCategoryId { get; set; }
    public required string Type { get; set; }
    public required string WhatToMeasure { get; set; }
    public required string Unit { get; set; }
    public required decimal Coefficient { get; set; }
    public TimeOnly? CutOff { get; set; }
}