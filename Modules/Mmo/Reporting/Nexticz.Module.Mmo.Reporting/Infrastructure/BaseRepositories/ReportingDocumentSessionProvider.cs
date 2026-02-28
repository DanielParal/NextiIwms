using Nexticz.Module.Mmo.SharedKernel.DataAccess;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.BaseRepositories;

internal class ReportingDocumentSessionProvider(
    IReportingDocumentStore store) 
    : DocumentSessionProvider(store), IReportingDocumentSessionProvider
{
    
}