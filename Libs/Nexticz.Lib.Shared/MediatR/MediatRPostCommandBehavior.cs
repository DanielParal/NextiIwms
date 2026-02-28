using MediatR;
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Lib.Shared.MediatR;

public abstract class MediatRPostCommandBehavior<TRequest, TResponse, TRequestType>(
    IMartenUnitOfWork unitOfWork,
    IMediatRNotificationCollector notificationCollector
    )
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Execute the handler (and any inner behaviors) first
        var response = await next();

        // Validate if request is type of IPlanningCommand, IWashingCommand etc...
        if (request is TRequestType)
        {
            await ExecutePostProcessingForSubmoduleCommandsAsync(cancellationToken);
        }
        
        return response;
    }

    protected virtual async Task ExecutePostProcessingForSubmoduleCommandsAsync(CancellationToken cancellationToken)
    {
        await notificationCollector.PublishNotificationsAsync(cancellationToken);
        if (!unitOfWork.IsExplicitTransaction)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}