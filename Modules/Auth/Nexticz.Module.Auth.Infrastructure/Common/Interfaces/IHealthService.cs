using Refit;

namespace Nexticz.Module.Auth.Infrastructure.Common.Interfaces;

public interface IHealtService
{
    [Get("/_health")]
    Task CheckHealth();
}