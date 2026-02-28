using Nexticz.Lib.Shared.Contracts;
using Refit;

namespace Nexticz.Module.Auth.Infrastructure.Common.Interfaces;

public interface ILangApiService
{
    [Post("/translations")]
    Task CreateTranslations([Header("Authorization")] string jwtToken, CreateTranslationsRequest createTranslationsRequest);
}