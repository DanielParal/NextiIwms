using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationByTitle;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity;
using Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.CreateSpecialInformation;

internal class CreateSpecialInformationCommandHandler(
    ISender sender,
    ILogger<CreateSpecialInformationCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork) : IRequestHandler<CreateSpecialInformationCommand, ErrorOr<SpecialInformation>>
{
    public async Task<ErrorOr<SpecialInformation>> Handle(CreateSpecialInformationCommand request, CancellationToken cancellationToken)
    {
        var existingSpecialInformation = await sender.Send(new GetSpecialInformationByTitleQuery(request.Title), cancellationToken);

        if (existingSpecialInformation.HasValue())
        {
            logger.LogInformation("Object {ObjectName} with title {Title} already exists. Nothing to create.", 
                nameof(SpecialInformation), request.Title);
            return SpecialInformationErrors.ValidationSpecialInformationWithTitleAlreadyExists;
        }
        
        var specialInformation = new SpecialInformation(request.Title.Trim(), request.Description, false);
        var specialInformationCreatedEvent = new SpecialInformationCreatedEvent(specialInformation.Id,
            specialInformation.Title, specialInformation.Description);
        unitOfWork.StartStream<SpecialInformationCreatedEvent, SpecialInformation>(specialInformation.Id, specialInformationCreatedEvent);

        return specialInformation;
    }
}