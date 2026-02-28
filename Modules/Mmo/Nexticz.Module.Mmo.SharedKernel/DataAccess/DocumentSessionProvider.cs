using Marten;
using Nexticz.Lib.Shared.DataAccess.Marten;

namespace Nexticz.Module.Mmo.SharedKernel.DataAccess;

public class DocumentSessionProvider(
    IDocumentStore store) : MartenDocumentSessionProvider(store), IDocumentSessionProvider;