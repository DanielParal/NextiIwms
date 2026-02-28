using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Users.Queries;

public record GetUsersByNotificationQuery(ReceivableNotificationContract ReceivableNotification) : IRequest<UserResponse[]>;