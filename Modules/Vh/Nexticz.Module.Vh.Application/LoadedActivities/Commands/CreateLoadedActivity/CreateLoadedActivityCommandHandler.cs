using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivity;

public class CreateLoadedActivityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateLoadedActivityCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateLoadedActivityCommand command, CancellationToken cancellationToken)
    {
        var createdLoadedActivity = new LoadedActivity(
            command.LoadedActivity.Created,
            command.LoadedActivity.Start,
            command.LoadedActivity.WorkerCode,
            command.LoadedActivity.CenterCode,
            command.LoadedActivity.ActivityCode,
            command.LoadedActivity.DepositorCode,
            command.LoadedActivity.DepositorGroupCode,
            command.LoadedActivity.ActivitySystemType,
            command.LoadedActivity.ActivityType,
            command.LoadedActivity.ActivitySource,
            command.LoadedActivity.ActivityState,
            command.LoadedActivity.PickingPlaceHight
        )
        {
            Note = command.LoadedActivity.Note,
            LoadingDeviceId = command.LoadedActivity.LoadingDeviceId,
            ActivityCutOff = command.LoadedActivity.ActivityCutOff,
            Coefficient = command.LoadedActivity.Coefficient,
            Unit = command.LoadedActivity.Unit,
            Idd = command.LoadedActivity.Idd,
            Idi = command.LoadedActivity.Idi,
            Idp = command.LoadedActivity.Idp,
            Idt = command.LoadedActivity.Idt,
            ActivityName = command.LoadedActivity.ActivityName,
            PDoklad = command.LoadedActivity.PDoklad,
            SortKod = command.LoadedActivity.SortKod,
            LicenceKod = command.LoadedActivity.LicenceKod
        };

        unitOfWork.Add(createdLoadedActivity);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}