using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Drying.Application.Interfaces;

namespace Nexticz.Module.Mmo.Drying.Infrastructure.BaseRepositories;

internal class DryingUnitOfWork(IDryingDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : Module.Mmo.SharedKernel.DataAccess.UnitOfWork(documentSessionProvider, currentUserProvider), IDryingUnitOfWork;