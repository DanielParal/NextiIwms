using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.Sign.SharedKernel.DataAccess;

namespace Nexticz.Module.Sign.SharedKernel.MediatR;

public class PostCommandBehavior<TRequest, TResponse, TRequestType>(
    IUnitOfWork unitOfWork,
    INotificationCollector notificationCollector
    )
    : MediatRPostCommandBehavior<TRequest, TResponse, TRequestType>(unitOfWork, notificationCollector)
    where TRequest : IRequest<TResponse>;