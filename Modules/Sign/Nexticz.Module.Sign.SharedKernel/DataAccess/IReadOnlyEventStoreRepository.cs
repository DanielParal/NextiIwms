using Marten.Events;
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.DevExtreme;

namespace Nexticz.Module.Sign.SharedKernel.DataAccess;

public interface IReadOnlyEventStoreRepository : IMartenReadOnlyEventStoreRepository;