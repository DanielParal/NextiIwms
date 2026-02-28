using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.SystemActivities.Commands.CreateSystemActivity;

public class CreateSystemActivityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSystemActivityCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateSystemActivityCommand command, CancellationToken cancellationToken)
    {
        var systemActivity = new SystemActivity
        {
            Name = command.CreateSystemActivityRequest.Name,
            ActionCodeWms = command.CreateSystemActivityRequest.ActionCodeWms,
            SystemType = command.CreateSystemActivityRequest.SystemType,
            DepositorGroupId = command.CreateSystemActivityRequest.DepositorGroupId,
            ActivityCategoryId = command.CreateSystemActivityRequest.ActivityCategoryId,
            Type = command.CreateSystemActivityRequest.Type,
            WhatToMeasure = command.CreateSystemActivityRequest.WhatToMeasure,
            Unit = command.CreateSystemActivityRequest.Unit,
            Coefficient = command.CreateSystemActivityRequest.Coefficient,
            CutOff = command.CreateSystemActivityRequest.CutOff
        };

        unitOfWork.Add(systemActivity);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}