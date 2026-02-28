using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Sign.SharedKernel.DataAccess;

public abstract class ReadOnlyEventStoreRepository(IDocumentSessionProvider documentSessionProvider) : 
    MartenReadOnlyEventStoreRepository(documentSessionProvider), IReadOnlyEventStoreRepository;