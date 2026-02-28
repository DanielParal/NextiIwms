namespace Nexticz.Module.Lang.Presentation;

public static class ApiEndpoints
{
    private const string ApiBase = "/api/lang";
    public static class Groups
    {
        public static readonly string[] All = [Lang];

        public const string Lang = nameof(Lang);
    }
    public static class Languages
    {
        private const string Base = $"{ApiBase}/languages";

        public const string GetLanguages = $"{Base}";
        public const string GetLanguageByShortcut = $"{Base}/{{shortcut}}";
        public const string UpdateLanguage = $"{Base}/{{shortcut}}";
    }

    public static class Translations
    {
        private const string Base = $"{ApiBase}/translations";

        public const string GetTranslations = $"{Base}";
        public const string GetTranslationById = $"{Base}/{{id}}";
        public const string CreateTranslations = $"{Base}";
        public const string UpdateTranslation = $"{Base}/{{id}}";
        public const string RemoveTranslation = $"{Base}/{{id}}";
    }
}