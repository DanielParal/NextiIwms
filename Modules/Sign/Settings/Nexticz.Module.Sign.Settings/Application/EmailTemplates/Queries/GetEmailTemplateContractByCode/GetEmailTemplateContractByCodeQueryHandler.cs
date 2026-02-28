using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;
using Nexticz.Module.Sign.Settings.Contracts.EmailTemplates.Queries;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateContractByCode;

internal class GetEmailTemplateContractByCodeQueryHandler(ISender sender) : IRequestHandler<GetEmailTemplateContractByCodeQuery, ErrorOr<EmailTemplateContract>>
{
    public async Task<ErrorOr<EmailTemplateContract>> Handle(GetEmailTemplateContractByCodeQuery request, CancellationToken cancellationToken)
    {
        var emailTemplate = await sender.Send(new GetEmailTemplateByCodeQuery(request.Code), cancellationToken);

        if (emailTemplate.IsError)
            return emailTemplate.Errors;
        
        return EmailTemplateContractFactory.Create(emailTemplate.Value);
    }
}