using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Module.Vh.Application.Centers.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.VhUsers.Commands;

public class UpdateVhUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateVhUserCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateVhUserCommand command, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.VhUsersRepository.GetVhUserByIdAsync(command.UpdateVhUserRequest.Id,
            cancellationToken);

        if (user is null) return VhUserErrors.VhUserWithIdDoesnotExist;

        var center =
            (await unitOfWork.CentersRepository.GetCentersAsync(new CentersFilteringParams(), cancellationToken)).data
            .OfType<Center>().ToList();

        user.Centers = center.Where(x => command.UpdateVhUserRequest.Centers.Contains(x.Code)).ToList();

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}