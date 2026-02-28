namespace Nexticz.Module.Notifications.Presentation;

internal static class NotificationsEndpoints
{
    private const string NotificationsBase = "/api/notifications";
    public static string GetOpenApiName(string sectionName) => NotificationsBase + sectionName;
    
    internal static class OpenApiContractEndpoints
    {
        private const string Base = $"{NotificationsBase}/openapicontracts";
        
        public const string GetOpenApiContracts = $"{Base}";
    }
    
    internal static class HubEndpoints
    {
        private const string Base = $"{NotificationsBase}";
        
        public const string SignalR = $"{Base}/signalr";
    }
}