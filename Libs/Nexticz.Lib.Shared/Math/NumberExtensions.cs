namespace Nexticz.Lib.Shared.Math;

public static class NumberExtensions
{
    public static bool IsOdd(this int value)
        => value % 2 != 0;

    public static bool IsEven(this int value)
        => value % 2 == 0;
}