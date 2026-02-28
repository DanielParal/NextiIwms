using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Queries.GetDocumentTemplateByCode;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Commands;

internal static class DepositorValidator
{
    public static async Task<ErrorOr<DepositorValidatorResult>> ValidateAsync(
        string code, string depositorGroupCode, string deliveryTemplateCode, string loadingTemplateCode,
        ISender sender, ILogger logger, CancellationToken cancellationToken)
    {
        var existingDepositorGroup = await sender.Send(new GetDepositorGroupByCodeQuery(depositorGroupCode), cancellationToken);
        if (!existingDepositorGroup.HasValue())
        {
            logger.LogInformation("Sign - Settings - depositor group with code: {Code} does not exists, DepositorCode: {DepositorCode}. Nothing to create",
                depositorGroupCode, code);
            return DepositorErrors.ValidationDepositorGroupDoesNotExist;
        }
        
        var existingDeliveryTemplateCode = await sender.Send(new GetDocumentTemplateByCodeQuery(deliveryTemplateCode), cancellationToken);
        if (!existingDeliveryTemplateCode.HasValue())
        {
            logger.LogInformation("Sign - Settings - delivery template with code: {Code} does not exists, DepositorCode: {DepositorCode}. Nothing to create",
                deliveryTemplateCode, code);
            return DepositorErrors.ValidationDeliveryTemplateDoesNotExist;
        }
        
        var existingLoadingTemplateCode = await sender.Send(new GetDocumentTemplateByCodeQuery(loadingTemplateCode), cancellationToken);
        if (!existingLoadingTemplateCode.HasValue())
        {
            logger.LogInformation("Sign - Settings - loading template with code: {Code} does not exists, DepositorCode: {DepositorCode}. Nothing to create",
                deliveryTemplateCode, code);
            return DepositorErrors.ValidationLoadingTemplateDoesNotExist;
        }

        return new DepositorValidatorResult(existingDepositorGroup.Value.Code, existingDeliveryTemplateCode.Value.Code, existingLoadingTemplateCode.Value.Code);
    }
}
internal record DepositorValidatorResult(string DepositorGroupCode, string DeliveryTemplateCode, string LoadingTemplateCode);