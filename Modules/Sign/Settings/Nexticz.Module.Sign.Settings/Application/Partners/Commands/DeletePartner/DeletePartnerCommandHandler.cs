using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiversByPartnerCode;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate.Events;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.DeletePartner;

internal class DeletePartnerCommandHandler(
    ILogger<DeletePartnerCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
) 
    : IRequestHandler<DeletePartnerCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePartnerCommand request, CancellationToken cancellationToken)
    {
        var partner = await sender.Send(new GetPartnerByCodeQuery(request.Code), cancellationToken);

        if (partner.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", nameof(Partner), request.Code);
            return PartnerErrors.ValidationCodeDoesNotExist;
        }
        
        var receivers = await sender.Send(new GetReceiversByPartnerCodeQuery(partner.Value.Code), cancellationToken);
        if (receivers.Length > 0)
        {
            var receiversCodes = string.Join(", ", receivers.Select(x => x.Code));
            logger.LogInformation("Sign - we cannot delete {ObjectName} with code: {Code} because it is used by receivers: {ReceiversCodes}.",
                nameof(Receiver), partner.Value.Code, receiversCodes);
            return PartnerErrors.ValidationCodeIsUsedInReceivers(receiversCodes);
        }
        
        var partnerDeletedEvent = new PartnerDeletedEvent(partner.Value.Id, request.Code);
        unitOfWork.AppendEvent(partner.Value.Id, partnerDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(Partner), request.Code);
        return Result.Deleted;
    }
}