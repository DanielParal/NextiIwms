namespace Nexticz.Module.Vh.Contracts.BandRewards;

public class UpdateBandRewardRequest
{
    public required string Band { get; set; }
    public required int BandNumber { get; set; }
    public required int MinValue { get; set; }
    public required int MaxValue { get; set; }
    public required int Reward { get; set; }
}