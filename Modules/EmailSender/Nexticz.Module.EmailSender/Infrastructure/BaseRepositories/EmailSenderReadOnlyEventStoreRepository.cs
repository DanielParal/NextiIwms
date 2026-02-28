using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.EmailSender.Application.Interfaces;

namespace Nexticz.Module.EmailSender.Infrastructure.BaseRepositories;

internal class EmailSenderReadOnlyEventStoreRepository(IEmailSenderDocumentSessionProvider sessionProvider) 
    : MartenReadOnlyEventStoreRepository(sessionProvider), IEmailSenderReadOnlyEventStoreRepository
{
    
}