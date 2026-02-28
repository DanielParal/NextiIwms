using System.Net;
using ErrorOr;
using Microsoft.Extensions.Logging;
using SharpIpp;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;

namespace Nexticz.Lib.Shared.PrintingUtils;

public abstract class PrintHandler(
    ILogger<PrintHandler> logger) : IPrintHandler
{
    public async Task<ErrorOr<Success>> PrintAsync(
        string printerIp,
        byte[] documentToPrint,
        int numberOfCopies,
        bool printTwoSides,
        string? userName,
        string? password,
        CancellationToken cancellationToken)
    {
        using var ippClient = CreateSharpIppClient(userName, password);

        var request = new PrintJobRequest
        {
            Document = new MemoryStream(documentToPrint),
            OperationAttributes = new PrintJobOperationAttributes
            {
                PrinterUri = new Uri($"http://{printerIp}:631/ipp/print")
            },
            JobTemplateAttributes = new JobTemplateAttributes
            {
                Copies = numberOfCopies,
                MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                Sides = printTwoSides ? Sides.TwoSidedLongEdge : null
            }
        };
        
        var response = await HandlePrintAsync(ippClient, request, cancellationToken);
        if (response.IsError)
            return response.Errors;

        return Result.Success;
    }
    
    private static SharpIppClient CreateSharpIppClient(string? userName, string? password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return new SharpIppClient();
        
        var credentials = new NetworkCredential
        {
            UserName = userName,
            Password = password
        };
        
        var handler = new HttpClientHandler
        {
            Credentials = credentials,
            PreAuthenticate = true
        };
        var httpClient = new HttpClient(handler);
        return new SharpIppClient(httpClient);
    }

    private async Task<ErrorOr<Success>> HandlePrintAsync(SharpIppClient ippClient, PrintJobRequest printJobRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await ippClient.PrintJobAsync(printJobRequest, cancellationToken);
            if ((int)response.StatusCode >= 0x0000 && (int)response.StatusCode <= 0x00FF)
            {
                logger.LogInformation(
                    "Printing - Print job submitted successfully. Job ID: {JobId}, job status: {StatusCode}",
                    response.JobId, response.StatusCode);
                return Result.Success;
            }

            logger.LogWarning(
                "Printing - Print job was not successfull. Job ID: {JobId}, job status: {StatusCode}, " +
                "job state message: {JobStateMessage}, job state reasons: {JobStateReasons}",
                response.JobId, response.StatusCode, response.JobStateMessage,
                string.Join(", ", response.JobStateReasons));
            return PrintingErrors.ValidationPrintNotSuccessful(string.Join(", ", response.JobStateReasons));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Printing - error printing. Error message: {ErrorMessage}", ex.Message);
            return PrintingErrors.ValidationUnExpectedError(ex.Message);
        }
    }
}