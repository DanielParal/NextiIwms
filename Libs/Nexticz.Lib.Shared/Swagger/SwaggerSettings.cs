
namespace Nexticz.Lib.Shared.Swagger;

public class SwaggerSettings
{
    public required string ApiName { get; set; }
    public required string ApiTitle { get; set; }
    public required string RoutePrefix { get; set; }
    public required List<SwaggerModule> Modules { get; set; }
}

public class SwaggerModule
{
    public string Name { get; set; }
    public List<string> SubModules { get; set; } = [];
}