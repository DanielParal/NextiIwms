namespace Nexticz.Module.Lang.Application.Common.Helpers;

public static class LangHelper
{
    public static void SplitComponentSlug(string slug, out string module, out string feature, out string component)
    {
        var splitedSlug = slug.Split('-');
        module = splitedSlug[0];
        feature = splitedSlug[1];
        component = splitedSlug[2];
    }
}