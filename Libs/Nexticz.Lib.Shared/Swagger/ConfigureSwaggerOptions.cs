using System.Runtime.CompilerServices;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nexticz.Lib.Shared.Swagger;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration config)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly SwaggerSettings _swaggerSettings = SwaggerSettingsFactory.Create(config);

    public void Configure(SwaggerGenOptions options)
    {
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            var version = apiDesc.GetApiVersion();
            var action = apiDesc.ActionDescriptor;
            var route = action.EndpointMetadata.OfType<IRouteDiagnosticsMetadata>().FirstOrDefault()?.Route;
            
            var moduleName = route?.Split("/")[2];
            
            var module = _swaggerSettings
                .Modules
                .FirstOrDefault(x => x.Name.Equals(moduleName, StringComparison.CurrentCultureIgnoreCase));

            var groupName = moduleName;
            if (module?.SubModules is not null &&
                module.SubModules.Count > 0)
            {
                groupName = $"{route?.Split("/")[2]}-{route?.Split("/")[3]}";
            }

            if (groupName == null || version == null) return false;

            return groupName.ToLowerInvariant() + "-" + version == docName;
        });

        var modules = _swaggerSettings
            .Modules
            .SelectMany(module =>
            {
                return module.SubModules.Count == 0 ? 
                    [module.Name] : 
                    module.SubModules.Select(subModule => $"{module.Name}-{subModule}");
            })
            .ToArray();

        foreach (var description in provider.ApiVersionDescriptions)
        foreach (var group in modules)
            options.SwaggerDoc($"{group.ToLowerInvariant()}-{description.GroupName}",
                CreateInfoForApiVersion(description));

        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = StringHelper.Header.XApiKey,
            Type = SecuritySchemeType.ApiKey,
            Description = "ApiKey Authentication. Enter your ApiKey."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });

        options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Basic",
            Description = "Basic Authentication. Enter your username and password.",
            In = ParameterLocation.Header
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Basic"
                    }
                },
                Array.Empty<string>()
            }
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please provide a valid token",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    public static void CreateSwaggerJsonFileUrls(WebApplication app, SwaggerUIOptions options)
    {
        var swaggerSettings = SwaggerSettingsFactory.Create(app.Configuration);
        var routePrefix = swaggerSettings.RoutePrefix;
        foreach (var description in app.DescribeApiVersions())
        foreach (var group in GetApiEndpointGroups(swaggerSettings.Modules.ToArray()))
        {
            options.SwaggerEndpoint(
                $"/swagger/{group.ToLowerInvariant()}-{description.ApiVersion}/swagger.json",
                $"{description.ApiVersion}-{group.ToLowerInvariant()}");
            options.DocumentTitle = swaggerSettings.ApiTitle;
            options.RoutePrefix = routePrefix;
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = _swaggerSettings.ApiName,
            Version = description.ApiVersion.ToString(),
            Description = "Magic APIs for your business",
            Contact = new OpenApiContact { Name = "podpora", Email = "podpora@nexti.cz" }
        };

        if (description.IsDeprecated) info.Description += " This API version has been deprecated.";

        return info;
    }
    
    private static string[] GetApiEndpointGroups(SwaggerModule[] swaggerModules)
    {
        var swaggerModuleNames = new List<string>();

        foreach (var swaggerModule in swaggerModules)
        {
            swaggerModuleNames.Add(swaggerModule.Name);

            var subModulesNames = swaggerModule.SubModules
                .Select(subModule => $"{swaggerModule.Name}-{subModule}");
            
            swaggerModuleNames.AddRange(subModulesNames);
        }
        
        return swaggerModuleNames.ToArray();
    }
}

public class RequiredNotNullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null) return;

        var properties = context.Type.GetProperties();

        foreach (var schemProp in schema.Properties)
        {
            var codeProp =
                properties.SingleOrDefault(x => x.Name.ToCamelCase() == schemProp.Key)
                ?? throw new MissingFieldException(
                    $"Could not find property {schemProp.Key} in {context.Type}, or several names conflict."
                );

            var isRequired = Attribute.IsDefined(codeProp, typeof(RequiredMemberAttribute));
            if (isRequired)
                //schemProp.Value.Nullable = false;
                _ = schema.Required.Add(schemProp.Key);
        }
    }
}