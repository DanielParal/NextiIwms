using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.Constants.Queries;
using Nexticz.Lib.Shared.Emails;
using Nexticz.Module.Sign.Settings.Application.Constants.Queries.GetConstantByKey;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Queries.GetIndividualConstantValues;

internal class GetEmailsForMissingRequiredDataDuringTransferConstantValueQueryHandler(
    ISender sender,
    ILogger<GetEmailsForMissingRequiredDataDuringTransferConstantValueQueryHandler> logger) 
    : IRequestHandler<GetEmailsForMissingRequiredDataDuringTransferConstantValueQuery, string[]>
{
    public async Task<string[]> Handle(GetEmailsForMissingRequiredDataDuringTransferConstantValueQuery request, CancellationToken cancellationToken)
    {
        var constant = 
            await sender.Send(new GetConstantByKeyQuery(ConstantNames.EmailsForMissingRequiredDataDuringTransfer), cancellationToken);
        
        if (constant.IsError)
        {
            logger.LogError("SIGN - Settings - Object {ObjectName} with key: {Key} does not exist. We cannot retrieve constant.", 
                nameof(Constant), ConstantNames.EmailsForMissingRequiredDataDuringTransfer);
            return [];
        }
        
        var emailsForMissingRequiredData = constant.Value.Value.Split(';');
        var emailsToReturn = new List<string>();

        foreach (var email in emailsForMissingRequiredData)
        {
            if (!EmailValidator.IsValidEmail(email))
            {
                logger.LogWarning("SIGN - Settings - Invalid email address: {InvalidEmail}. Skipping", email);
                continue;
            }
            
            emailsToReturn.Add(email);
        }

        if (emailsToReturn.Count == 0)
        {
            logger.LogWarning("SIGN - Settings - No valid email addresses found for constant: {ConstantName}. Returning empty array", ConstantNames.EmailsForMissingRequiredDataDuringTransfer);
        }
        
        return emailsToReturn.ToArray();
    }
}