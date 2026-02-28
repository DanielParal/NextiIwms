using Microsoft.Extensions.Configuration;

namespace Nexticz.Lib.Shared.BaseUrls;

public static class BaseUrlSettingsFactory
{
    public static BaseUrlSettings Create(IConfiguration configuration)
    {
        return configuration.GetSection(nameof(BaseUrlSettings)).Get<BaseUrlSettings>()
            ?? throw new InvalidOperationException("BaseUrl Settings not found.");
    }
}