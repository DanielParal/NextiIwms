using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Commands.DeleteConstant;

internal class DeleteConstantCommandHandler(
    ILogger<DeleteConstantCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender
    ) : IRequestHandler<DeleteConstantCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteConstantCommand request, CancellationToken cancellationToken)
    {
        var constant = await sender.Send(new GetConstantByKeyQuery(request.Key), cancellationToken);

        if (constant.IsError)
        {
            logger.LogInformation("Did not find object {ObjectName} with key: {Key}. Nothing to delete.", 
                nameof(Constant), request.Key);
            return constant.Errors;
        }
        
        var constantDeletedEvent = new ConstantDeletedEvent(constant.Value.Id, request.Key);
        unitOfWork.AppendEvent(constant.Value.Id, constantDeletedEvent);
        return Result.Deleted;
    }
}