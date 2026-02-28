using Marten;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.SharedKernel.DataAccess;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.BaseRepositories;

internal class ReportingReadOnlyEventStoreRepository(IReportingDocumentSessionProvider documentSessionProvider) 
    : ReadOnlyEventStoreRepository(documentSessionProvider), IReportingReadOnlyEventStoreRepository;