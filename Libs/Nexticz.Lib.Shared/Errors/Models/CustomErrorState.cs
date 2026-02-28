namespace Nexticz.Lib.Shared.Errors.Models;

public class CustomErrorState(object error, params string[]? parameters)
{
    public object Error { get; set; } = error;
    public string[]? Parameters { get; set; } = parameters;
}