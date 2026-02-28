using MediatR;
using Nexticz.Lib.Shared.MediatR;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.SharedKernel.MediatR;

public class PostCommandBehavior<TRequest, TResponse, TRequestType>(
    IUnitOfWork unitOfWork,
    INotificationCollector notificationCollector
    )
    : MediatRPostCommandBehavior<TRequest, TResponse, TRequestType>(unitOfWork, notificationCollector), IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>;