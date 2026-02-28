
using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity;
using Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.CreateManufacture;

internal class CreateManufactureCommandHandler(
    ILogger<CreateManufactureCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender)
    : IRequestHandler<CreateManufactureCommand, ErrorOr<Manufacture>>
{
    public async Task<ErrorOr<Manufacture>> Handle(CreateManufactureCommand request, CancellationToken cancellationToken)
    {
        var existingManufacture = await sender.Send(new GetManufactureByCodeQuery(request.Code), cancellationToken);
        if (existingManufacture.HasValue())
        {
            logger.LogInformation("Mmo - Object {ObjectName} with code: {Code} already exists. Nothing to create.", nameof(Manufacture), request.Code);
            return ManufactureErrors.ValidationCodeAlreadyExists(request.Code);
        }
        
        var manufacture = new Manufacture(request.Code, request.Name);
        var manufactureCreatedEvent = new ManufactureCreatedEvent(manufacture.Id, manufacture.Code, manufacture.Name);
        
        unitOfWork.StartStream<ManufactureCreatedEvent, Manufacture>(manufacture.Id, manufactureCreatedEvent);
        
        logger.LogInformation("Mmo - Object {ObjectName} with code: {Code} created.", nameof(Manufacture), request.Code);
        
        return manufacture;
    }
}