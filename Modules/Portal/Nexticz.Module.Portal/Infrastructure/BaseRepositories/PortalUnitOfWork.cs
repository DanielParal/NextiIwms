using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Portal.Application.Interfaces;

namespace Nexticz.Module.Portal.Infrastructure.BaseRepositories;

internal class PortalUnitOfWork(IPortalDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : MartenUnitOfWork(documentSessionProvider, currentUserProvider), IPortalUnitOfWork;