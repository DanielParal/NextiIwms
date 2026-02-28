using System.Text;

namespace Nexticz.OnPremise.Monitoring.Exe.Utils;

internal sealed class Base62IdGenerator
{
    private static readonly char[] Base62Chars =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            .ToCharArray();

    public static string GenerateId(int length)
    {
        var sb = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            sb.Append(Base62Chars[new Random().Next(62)]);
        }

        return sb.ToString();
    }
}