using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;

namespace Nexticz.Module.Sign.SharedKernel.DataAccess;

public abstract class UnitOfWork(IDocumentSessionProvider documentSessionProvider,
    ICurrentUserProvider currentUserProvider) : MartenUnitOfWork(documentSessionProvider, currentUserProvider), IUnitOfWork;