namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;

internal class LineItemQueueResponse(
    List<LineItemToCreateOrUpdate> lineItemsToCreateOrUpdate,
    LineItemToShorten? lineItemToShorten)
{
    public List<LineItemToCreateOrUpdate> LineItemsToCreateOrUpdate => lineItemsToCreateOrUpdate.OrderBy(x => x.StartDate).ToList();
    public List<LineItemToCreateOrUpdate> LineItemsToCreate => lineItemsToCreateOrUpdate.Where(x => x.ExistingId is null).ToList();
    public List<LineItemToCreateOrUpdate> LineItemsToUnplan => lineItemsToCreateOrUpdate.Where(x => x.ExistingId is not null).ToList();
    public LineItemToShorten? LineItemToShorten => lineItemToShorten;

    public void AddLineItemsToCreateOrUpdate(List<LineItemToCreateOrUpdate> lineItemsToAdd)
    {
        lineItemsToCreateOrUpdate.AddRange(lineItemsToAdd);
    }
}