using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands;

internal static class ReceiverValidator
{
    public static async Task<ErrorOr<ReceiverValidatorResult>> ValidateAsync(string partnerCode, ISender sender, ILogger logger, CancellationToken cancellationToken)
    {
        var existingPartnerCode = await sender.Send(new GetPartnerByCodeQuery(partnerCode), cancellationToken);
        if (existingPartnerCode.IsError)
        {
            logger.LogInformation("Sign - partner with code: {Code} does not exists. Nothing to create.",
                partnerCode);
            return ReceiverErrors.ValidationPartnerDoesNotExist;
        } 

        return new ReceiverValidatorResult(existingPartnerCode.Value.Code);
    }
}

internal record ReceiverValidatorResult(string PartnerCode);