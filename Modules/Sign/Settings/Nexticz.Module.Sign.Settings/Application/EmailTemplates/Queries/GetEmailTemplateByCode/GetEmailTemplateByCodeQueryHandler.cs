using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Queries.GetEmailTemplateByCode;

internal class GetEmailTemplateByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetEmailTemplateByCodeQuery, ErrorOr<EmailTemplate>>
{
    public async Task<ErrorOr<EmailTemplate>> Handle(GetEmailTemplateByCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.Code.ToUpperInvariant();
        
        var emailTemplate = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<EmailTemplate>(x => x.Code == upperCode, cancellationToken);

        if (emailTemplate is null)
            return EmailTemplateErrors.EmailTemplateNotFound;
        
        return emailTemplate;
    }
}