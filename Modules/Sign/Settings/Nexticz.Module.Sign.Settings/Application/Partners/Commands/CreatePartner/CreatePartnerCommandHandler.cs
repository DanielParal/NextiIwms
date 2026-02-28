using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.CreatePartner;

internal class CreatePartnerCommandHandler(
        ILogger<CreatePartnerCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreatePartnerCommand, ErrorOr<Partner>>
{
    public async Task<ErrorOr<Partner>> Handle(CreatePartnerCommand request, CancellationToken cancellationToken)
    {
        var existingPartner = await sender.Send(new GetPartnerByCodeQuery(request.Code), cancellationToken);

        if (existingPartner.HasValue())
        {
            logger.LogInformation("Sign - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(Partner), request.Code);
            return PartnerErrors.ValidationCodeAlreadyExists;
        }
            
        var partner = new Partner(request.Code, request.Name);
        var partnerCreatedEvent =
            new PartnerCreatedEvent(partner.Id, partner.Code, partner.Name);

        unitOfWork.StartStream<PartnerCreatedEvent, Partner>(partner.Id, partnerCreatedEvent);
            
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} created.",
            nameof(Partner), request.Code);
        return partner;
    }
}