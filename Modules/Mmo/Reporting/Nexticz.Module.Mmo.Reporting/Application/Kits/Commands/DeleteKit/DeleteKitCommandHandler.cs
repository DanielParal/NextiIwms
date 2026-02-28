using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Time;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLineItemById;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.DeleteKit;

internal class DeleteKitCommandHandler(
    ISender sender,
    IReportingUnitOfWork unitOfWork,
    ILogger<DeleteKitCommandHandler> logger,
    IClock clock,
    ICurrentUserProvider currentUserProvider) : IRequestHandler<DeleteKitCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteKitCommand request, CancellationToken cancellationToken)
    {
        var lineItem = await sender.Send(new GetLineItemByIdQuery(request.Id), cancellationToken);
        
        if (lineItem.IsError)
        {
            logger.LogWarning("Reporting - line item does not exist. We cannot delete kit. Id: {Id}.", request.Id);
            return lineItem.Errors;
        }

        var userName = currentUserProvider.GetCurrentUser().UserName;
        var kitDeletedEvent = new KitDeletedEvent(request.Id, clock.UtcNowOffset, userName);
        unitOfWork.AppendEvent(request.Id, kitDeletedEvent);
        
        logger.LogInformation("Reporting - kit deleted. Id: {Id}.", request.Id);
        return Result.Success;
    }
}