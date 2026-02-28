namespace Nexticz.Module.Auth.Presentation;

public static class ApiEndpoints
{
    private const string ApiBase = "/api/auth";

    public static class Groups
    {
        public const string Auth = nameof(Auth);
        public static readonly string[] All = [Auth];
    }

    public static class Authentications
    {
        private const string Base = $"{ApiBase}/authentications";

        public const string Register = $"{Base}/register";
        public const string RegisterPasswordless = $"{Base}/registerPasswordless";
        public const string EmailConfirmation = $"{Base}/emailConfirmation";
        public const string ConfirmEmailCompletion = $"{Base}/confirmEmailCompletion/{{jwtToken}}";
        public const string GetAccountInfo = $"{Base}/getAccountInfo";
        public const string Login = $"{Base}/login";
        public const string LoginPasswordless = $"{Base}/loginPasswordless";
        public const string Logout = $"{Base}/logout";
        public const string ResetPassword = $"{Base}/resetPassword";
        public const string ResetPasswordCompletion = $"{Base}/resetPasswordCompletion/{{jwtToken}}";
        public const string GetAccessTokenForApiKey = $"{Base}/getAccessTokenForApiKey";
        public const string RefreshAccessToken = $"{Base}/refreshAccessToken";
        public const string GetLoggedUserInfo = $"{Base}/getLoggedUserInfo";
        public const string ForgottenPassword = $"{Base}/forgottenPassword";
    }

    public static class Accounts
    {
        private const string Base = $"{ApiBase}/accounts";

        public const string GetAccounts = $"{Base}";
        public const string GetAccountByUsernameOrId = $"{Base}/{{usernameOrId}}";
        public const string CreateAccount = $"{Base}";
        public const string UpdateAccount = $"{Base}/{{id}}";
        public const string DeleteAccount = $"{Base}/{{id}}";
        public const string SyncAccountsInModules = $"{Base}/syncedmodules";
    }

    public static class ApiKeys
    {
        private const string Base = $"{ApiBase}/apiKeys";

        public const string GetApiKeysByUserId = $"{Base}/{{userId}}";
        public const string CreateApiKey = $"{Base}";
        public const string UpdateApiKey = $"{Base}/{{id}}";
        public const string DeleteApiKey = $"{Base}/{{id}}";
    }

    public static class RefreshTokens
    {
        private const string Base = $"{ApiBase}/refreshTokens";

        public const string GetRefreshTokensByUserId = $"{Base}/{{userId}}";
        public const string DeleteRefreshToken = $"{Base}/{{id}}";
    }
    
    public static class Me
    {
        private const string Base = $"{ApiBase}/me";

        public const string GetMe = $"{Base}";
    }
}