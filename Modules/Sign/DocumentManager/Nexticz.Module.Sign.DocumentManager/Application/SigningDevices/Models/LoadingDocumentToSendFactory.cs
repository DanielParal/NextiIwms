using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;

internal static class LoadingDocumentToSendFactory
{
    public static LoadingDocumentToSend[] Create(SendDocumentJobContract[] sendDocumentJobContracts)
    {
        return sendDocumentJobContracts
            .GroupBy(contract => contract.LoadingDocumentCode)
            .Select(group => new LoadingDocumentToSend(
                group.Key,
                group.Select(contract => contract.DeliveryDocumentCode).Where(code => !string.IsNullOrWhiteSpace(code)).ToArray()!,
                group.Any(contract => contract.DeliveryDocumentCode is null)))
            .ToArray();
    }
}