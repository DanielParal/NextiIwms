using System.Net;
using Nexticz.Module.Mmo.Settings.Contracts.Exports;
using Nexticz.Module.Mmo.Settings.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;

using Shouldly;

namespace Nexticz.Module.Mmo.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class ExportApiTests
{
    private readonly HttpClient _client;

    public ExportApiTests(SettingsApiFactoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateExport_ShouldReturnUnauthorized_WhenNotPassingAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ExportEndpoints.CreateExport)
                .WithContent(new
                {
                    ExportType = "PackagingXlsx"
                })
                .SendAsync(); 
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateExport_ShouldReturnForbidden_WhenPassingMemberAuthToken()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ExportEndpoints.CreateExport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyMember)
                .WithContent(new
                {
                    ExportType = "PackagingXlsx"
                })
                .SendAsync(); 
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task CreateExport_ShouldFail_WhenPassingNotValidExportType()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ExportEndpoints.CreateExport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    ExportType = "NonExisting"
                })
                .SendAsync(); 
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GetExports_ShouldReturnSomething_WhenAtLeastOneExportCreated()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.ExportEndpoints.CreateExport)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    ExportType = "PackagingXlsx"
                })
                .SendAsync(); 
        
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.ExportEndpoints.GetExports}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<ExportResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.GetType().ShouldBe(typeof(FilteredResult<ExportResponse>));
        getResponse.responseContent.Data.Count.ShouldBeGreaterThan(0);
    }
}