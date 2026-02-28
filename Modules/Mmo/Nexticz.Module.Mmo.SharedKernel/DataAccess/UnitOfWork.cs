using Marten;
using Marten.Services;
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;
using Npgsql;

namespace Nexticz.Module.Mmo.SharedKernel.DataAccess;

public abstract class UnitOfWork(IDocumentSessionProvider documentSessionProvider,
    ICurrentUserProvider currentUserProvider) : MartenUnitOfWork(documentSessionProvider, currentUserProvider), IUnitOfWork;