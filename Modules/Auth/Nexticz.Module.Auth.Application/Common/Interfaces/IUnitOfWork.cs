using Nexticz.Lib.Shared.DataAccess.EntityFramework;

namespace Nexticz.Module.Auth.Application.Common.Interfaces;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IAppUserRefreshTokensRepository AppUserRefreshTokensRepository { get; }
    IAppUserApiKeysRepository AppUserApiKeysRepository { get; }
}