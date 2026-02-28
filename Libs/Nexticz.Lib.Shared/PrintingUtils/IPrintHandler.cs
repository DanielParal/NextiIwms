using ErrorOr;

namespace Nexticz.Lib.Shared.PrintingUtils;

public interface IPrintHandler
{
    Task<ErrorOr<Success>> PrintAsync(string printerIp, byte[] documentToPrint, int numberOfCopies, bool printTwoSides, string? userName, string? password, CancellationToken cancellationToken);
}