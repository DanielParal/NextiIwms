using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentLoader.Infrastructure.BaseRepositories;

internal class DocumentLoaderUnitOfWork(IDocumentLoaderDocumentSessionProvider documentSessionProvider, ICurrentUserProvider currentUserProvider)
    : Module.Sign.SharedKernel.DataAccess.UnitOfWork(documentSessionProvider, currentUserProvider), IDocumentLoaderUnitOfWork;