using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.UpdatePartner;

internal class UpdatePartnerCommandHandler(
    ILogger<UpdatePartnerCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdatePartnerCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePartnerCommand request, CancellationToken cancellationToken)
    {
        var partner = await sender.Send(new GetPartnerByCodeQuery(request.Code), cancellationToken);

        if (partner.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(Partner), request.Code);
            return PartnerErrors.ValidationCodeDoesNotExist;
        }
        
        var partnerUpdatedEvent = new PartnerUpdatedEvent(partner.Value.Id, request.Code, request.Name);
        unitOfWork.AppendEvent(partner.Value.Id, partnerUpdatedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code}, name: {Name} updated.",
            nameof(Partner), request.Code, request.Name);
        return Result.Updated;
    }
}