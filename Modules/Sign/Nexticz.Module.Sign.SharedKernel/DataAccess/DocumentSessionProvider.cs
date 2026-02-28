using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Lib.Shared.UserProviders;

namespace Nexticz.Module.Sign.SharedKernel.DataAccess;

public class DocumentSessionProvider(
    IDocumentStore store) : MartenDocumentSessionProvider(store), IDocumentSessionProvider;