using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Users.Queries;

public record GetUserResponsesByDepositorAndGroupCodesQuery(string[] DepositorCodes, string[] DepositorGroupCodes) : IRequest<UserResponse[]>;