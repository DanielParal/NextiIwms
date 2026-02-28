using System.Text.Json.Serialization;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailTemplates;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmailTemplateTypeContract
{
    LoadingDocumentTemplate,
    DeliveryDocumentTemplate
}