using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.CreateNonDispensingActivity;

public class CreateNonDispensingActivityCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateNonDispensingActivityCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateNonDispensingActivityCommand command, CancellationToken cancellationToken)
    {
        var createdNonDispensingActivity = new NonDispensingActivity
        {
            ActivityIdentifier = command.CreateNonDispensingActivityRequest.ActivityIdentifier,
            Name = command.CreateNonDispensingActivityRequest.Name,
            RequireNote = command.CreateNonDispensingActivityRequest.RequireNote,
            Note = command.CreateNonDispensingActivityRequest.Note,
            Unit = command.CreateNonDispensingActivityRequest.Unit,
            Coefficient = command.CreateNonDispensingActivityRequest.Coefficient,
            CutOff = command.CreateNonDispensingActivityRequest.CutOff,
            CenterId = command.CreateNonDispensingActivityRequest.CenterId,
            ActivityCategoryId = command.CreateNonDispensingActivityRequest.ActivityCategoryId,
        };
        
        unitOfWork.Add(createdNonDispensingActivity);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}