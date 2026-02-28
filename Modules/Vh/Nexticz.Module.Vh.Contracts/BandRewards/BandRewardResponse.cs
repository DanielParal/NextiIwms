namespace Nexticz.Module.Vh.Contracts.BandRewards;

public class BandRewardResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Band { get; set; }
    public required int BandNumber { get; set; }
    public required int MinValue { get; set; }
    public required int MaxValue { get; set; }
    public required int Reward { get; set; }
}