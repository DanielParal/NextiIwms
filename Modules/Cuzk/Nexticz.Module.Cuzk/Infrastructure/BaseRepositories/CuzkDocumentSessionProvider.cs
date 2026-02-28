using Nexticz.Lib.Shared.DataAccess.Marten;
using Nexticz.Module.Cuzk.Application.Interfaces;

namespace Nexticz.Module.Cuzk.Infrastructure.BaseRepositories;

internal class CuzkDocumentSessionProvider(
    ICuzkDocumentStore store) 
    : MartenDocumentSessionProvider(store), ICuzkDocumentSessionProvider
{
    
}