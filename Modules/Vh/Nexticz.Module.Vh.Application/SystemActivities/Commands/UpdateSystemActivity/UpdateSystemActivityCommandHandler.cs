using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.SystemActivities.Commands.UpdateSystemActivity;

public class UpdateSystemActivityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSystemActivityCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateSystemActivityCommand command, CancellationToken cancellationToken)
    {
        var systemActivity =
            await unitOfWork.SystemActivitiesRepository.GetSystemActivityByIdAsync(command.Id, cancellationToken);

        if (systemActivity is null) return SystemActivityErrors.SystemActivityWithIdDoesnotExist;

        systemActivity.Name = command.UpdateSystemActivityRequest.Name;
        systemActivity.ActionCodeWms = command.UpdateSystemActivityRequest.ActionCodeWms;
        systemActivity.SystemType = command.UpdateSystemActivityRequest.SystemType;
        systemActivity.DepositorGroupId = command.UpdateSystemActivityRequest.DepositorGroupId;
        systemActivity.ActivityCategoryId = command.UpdateSystemActivityRequest.ActivityCategoryId;
        systemActivity.Type = command.UpdateSystemActivityRequest.Type;
        systemActivity.WhatToMeasure = command.UpdateSystemActivityRequest.WhatToMeasure;
        systemActivity.Unit = command.UpdateSystemActivityRequest.Unit;
        systemActivity.Coefficient = command.UpdateSystemActivityRequest.Coefficient;
        systemActivity.CutOff = command.UpdateSystemActivityRequest.CutOff;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}