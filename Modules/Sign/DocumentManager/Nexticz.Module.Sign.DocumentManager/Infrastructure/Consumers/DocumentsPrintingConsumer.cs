using MassTransit;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.MassTransit;
using Nexticz.Module.Sign.DocumentManager.Application.PrintingHandling;
using Nexticz.Module.Sign.DocumentManager.Contracts.Printings;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Consumers;

internal class DocumentsPrintingConsumer(
    IDocumentsPrintingOrchestrator documentsPrintingOrchestrator,
    ILogger<DocumentsPrintingConsumer> logger) : IConsumer<DocumentsPrintingRequested>
{
    public async Task Consume(ConsumeContext<DocumentsPrintingRequested> context)
    {
        var currentRetryAttempt = context.GetRetryAttempt();
        logger.LogInformation("[Start] [DocumentsPrintingConsumer] current attempt: {CurrentRetryAttempt}", currentRetryAttempt);
        
        var isLastAttempt = currentRetryAttempt == DocumentsPrintingConsumerDefinition.RetryCount;
        
        var result = await documentsPrintingOrchestrator.OrchestrateAsync(context.Message, isLastAttempt, CancellationToken.None);

        if (result.IsError && !isLastAttempt)
        {
            logger.LogError("[End] [DocumentsPrintingConsumer] printing failed. Throwing exception for retry");
            throw new HandleLaterException(result.Errors.First().Description);
        }
        
        logger.LogInformation("[End] [DocumentsPrintingConsumer] isLastAttempt: {isLastAttempt}", isLastAttempt);
    }
}