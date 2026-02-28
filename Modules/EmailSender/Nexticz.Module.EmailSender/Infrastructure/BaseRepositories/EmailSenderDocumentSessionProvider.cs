using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.EmailSender.Application.Interfaces;

namespace Nexticz.Module.EmailSender.Infrastructure.BaseRepositories;

internal class EmailSenderDocumentSessionProvider(
    IEmailSenderDocumentStore store) 
    : MartenDocumentSessionProvider(store), IEmailSenderDocumentSessionProvider
{
    
}