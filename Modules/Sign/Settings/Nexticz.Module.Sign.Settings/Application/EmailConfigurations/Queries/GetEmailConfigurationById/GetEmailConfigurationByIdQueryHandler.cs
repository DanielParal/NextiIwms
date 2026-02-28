using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationById;

internal class GetEmailConfigurationByIdQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetEmailConfigurationByIdQuery, ErrorOr<EmailConfiguration>>
{
    public async Task<ErrorOr<EmailConfiguration>> Handle(GetEmailConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var emailConfiguration = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<EmailConfiguration>(
                x => x.Id == request.Id, 
                cancellationToken);

        if (emailConfiguration is null)
            return EmailConfigurationErrors.IdNotFound;
        
        return emailConfiguration;
    }
}