using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten;


namespace Nexticz.Module.Mmo.SharedKernel.DataAccess;

public abstract class ReadOnlyEventStoreRepository(IDocumentSessionProvider documentSessionProvider) : MartenReadOnlyEventStoreRepository(documentSessionProvider), IReadOnlyEventStoreRepository;