using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries;

internal static class LineItemForReviewFilter
{
    public static LineItemView[] FilterLineItemsForReview(LineItemView[] lineItems)
    {
        var downtimes = 
            lineItems.Where(x => x.Type is LineItemType.Downtime).ToArray();
        
        var lineItemsForReview = new List<LineItemView>();
        
        var downtimesWithoutReason = downtimes.Where(x => x.InactivityReasonId is null).ToArray();
        var downtimesWithReasonWithCommentNeeded = 
            downtimes
                .Where(x => x.InactivityReasonId is not null && x.IsCommentNeededForReview && string.IsNullOrWhiteSpace(x.Comment))
                .ToArray();
        
        lineItemsForReview.AddRange(downtimesWithoutReason);
        lineItemsForReview.AddRange(downtimesWithReasonWithCommentNeeded);
        
        return lineItemsForReview.ToArray();
    }
}