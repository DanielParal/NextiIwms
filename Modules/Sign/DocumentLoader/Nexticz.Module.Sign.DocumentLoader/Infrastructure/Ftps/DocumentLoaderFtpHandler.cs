using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexticz.Lib.Shared.Ftps;
using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;

namespace Nexticz.Module.Sign.DocumentLoader.Infrastructure.Ftps;

internal class DocumentLoaderFtpHandler(
    IOptions<TransferDocumentsFtpSettings> ftpSettings,
    ILogger<DocumentLoaderFtpHandler> logger) : FtpHandler(ftpSettings, logger), IDocumentLoaderFtpHandler;