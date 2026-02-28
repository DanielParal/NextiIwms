using Nexticz.Module.Lang.Application.Languages;
using Nexticz.Module.Lang.Application.Translations;
using Nexticz.Lib.Shared.DataAccess.EntityFramework;

namespace Nexticz.Module.Lang.Application.Common.Interfaces;

public interface IUnitOfWork : IBaseUnitOfWork
{
    ILanguagesRepository LanguageRepository { get; }
    ITranslationsRepository TranslationRepository { get; }
}