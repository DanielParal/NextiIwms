namespace Nexticz.Lib.Shared.Helpers;

public static class StringHelper
{
    public static class Claim
    {
        public static class Type
        {
            public const string MagicRoles = "magicRoles";
            public const string MagicPermissions = "magicPermissions";
            public const string MagicId = "magicId";
            public const string MagicEmail = "magicemail";
            public const string MagicUniqueName = "magicuniquename";
            public const string MagicFirstName = "magicfirstname";
            public const string MagicFamilyName = "magicfamilyname";
        }
    }

    public static class Header
    {
        public const string Authorization = "Authorization";
        public const string XApiKey = "X-api-key";
        public const string XAuthorizationFailed = "X-authorization-failed";
        public const string XAccessToken = "X-access-token";
        public const string XRefreshToken = "X-refresh-token";
        public const string XApiVersion = "X-api-version";
        public const string XApiLanguage = "X-api-language";
        public const string XDeploymentTimestamp = "X-Deployment-Timestamp";
        public const string XTenantTimeZone = "X-Tenant-TimeZone";
    }

    public static class AuthenticationSchema
    {
        public const string ApiKeySchema = "ApiKeySchema";
        public const string BasicAuthenticationSchema = "BasicAuthenticationSchema";
        public const string JwtBearerSchema = "JwtBearerSchema";
        public const string JwtBearerWithRefreshTokenInHttpOnlyCookieSchema = "JwtBearerWithRefreshTokenInHttpOnlyCookieSchema";
    }
}