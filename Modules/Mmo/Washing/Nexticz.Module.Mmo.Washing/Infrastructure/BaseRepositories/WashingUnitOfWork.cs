using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.BaseRepositories;

internal class WashingUnitOfWork(IWashingDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : UnitOfWork(documentSessionProvider, currentUserProvider), IWashingUnitOfWork;