namespace Nexticz.Lib.Shared.MassTransit;


public class HandleLaterException : Exception
{
    public HandleLaterException() : base()
    {
    }

    public HandleLaterException(string? message) : base(message)
    {
    }

    public HandleLaterException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}