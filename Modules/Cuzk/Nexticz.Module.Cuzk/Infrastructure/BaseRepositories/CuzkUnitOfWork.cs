using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Cuzk.Application.Interfaces;

namespace Nexticz.Module.Cuzk.Infrastructure.BaseRepositories;

internal class CuzkUnitOfWork(ICuzkDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : MartenUnitOfWork(documentSessionProvider, currentUserProvider), ICuzkUnitOfWork;