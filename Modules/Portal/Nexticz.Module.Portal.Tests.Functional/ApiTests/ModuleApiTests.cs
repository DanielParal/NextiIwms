
using System.Net;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Module.Portal.Presentation;
using Nexticz.Lib.Shared.DevExtreme;
using Shouldly;

namespace Nexticz.Module.Portal.Tests.Functional.ApiTests;

[Collection(nameof(PortalApiCollection))]
public class ModuleApiTests(PortalApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();

    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteModule_WhenCallingAllEndpointsOneByOne()
    {
        var name = Guid.NewGuid().ToString();
        var icon = Guid.NewGuid().ToString();
        var baseUrl = Guid.NewGuid().ToString();
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, PortalEndpoints.ModuleEndpoints.CreateModule)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateModuleRequest(
                    name, icon, baseUrl, true))
                .SendAndDeserializeAsync<ModuleResponse>();
        
        var createdId = createdResponse.responseContent!.Id;
        await VerifyModuleAsync(createdId, name, icon, baseUrl, true);
        
        var updateName = Guid.NewGuid().ToString();
        var updateIcon = Guid.NewGuid().ToString();
        var updateBaseUrl = Guid.NewGuid().ToString();
        var updateIsActive = false;
        var updateBody = new UpdateModuleRequest(updateName, updateIcon, updateBaseUrl, updateIsActive);
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{PortalEndpoints.ModuleEndpoints.GetModules}/{createdId}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyModuleAsync(createdId, updateName, updateIcon, updateBaseUrl, updateIsActive);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{PortalEndpoints.ModuleEndpoints.GetModules}/{createdId}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        await VerifyNotFoundAsync(createdId);
    }
    
    [Fact]
    public async Task ChangeOrder_ShouldChangeOrder_WhenMoveToFirstPosition()
    {
        var createdModule1 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, PortalEndpoints.ModuleEndpoints.CreateModule)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateModuleRequest(
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true))
                .SendAndDeserializeAsync<ModuleResponse>();
        
        var createdModule2 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, PortalEndpoints.ModuleEndpoints.CreateModule)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateModuleRequest(
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true))
                .SendAndDeserializeAsync<ModuleResponse>();
        
        var createdModule3 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, PortalEndpoints.ModuleEndpoints.CreateModule)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateModuleRequest(
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true))
                .SendAndDeserializeAsync<ModuleResponse>();
        
        var createdModule4 = await
            new HttpRequestBuilder(_client, HttpMethod.Post, PortalEndpoints.ModuleEndpoints.CreateModule)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new CreateModuleRequest(
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true))
                .SendAndDeserializeAsync<ModuleResponse>();
        
        // Act
         await new HttpRequestBuilder(_client, HttpMethod.Post, 
                 $"{PortalEndpoints.ModuleEndpoints.GetModules}/{createdModule3.responseContent!.Id}/orderChanges")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new ChangeModuleOrderRequest(1))
                .SendAsync();
         
         // Assert
         var getResponse = await
             new HttpRequestBuilder(_client, HttpMethod.Get, 
                     $"{PortalEndpoints.ModuleEndpoints.GetModules}")
                 .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                 .SendAndDeserializeAsync<FilteredResult<ModuleResponse>>();
         
         var allModules = getResponse.responseContent!.Data;
         
         allModules.Count.ShouldBe(4);
         allModules.First(x => x.Id == createdModule3.responseContent!.Id).SortOrder.ShouldBe(1);
         allModules.First(x => x.Id == createdModule1.responseContent!.Id).SortOrder.ShouldBe(2);
         allModules.First(x => x.Id == createdModule2.responseContent!.Id).SortOrder.ShouldBe(3);
         allModules.First(x => x.Id == createdModule4.responseContent!.Id).SortOrder.ShouldBe(4);
    }

    
    private async Task VerifyModuleAsync(Guid id, string expectedName, string expectedIcon, string expectedBaseUrl, bool isActive)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{PortalEndpoints.ModuleEndpoints.GetModules}/{id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ModuleResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Name.ShouldBe(expectedName);
        getResponse.responseContent.Icon.ShouldBe(expectedIcon);
        getResponse.responseContent.BaseUrl.ShouldBe(expectedBaseUrl);
        getResponse.responseContent.IsActive.ShouldBe(isActive);
    }
    
    private async Task VerifyNotFoundAsync(Guid id)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{PortalEndpoints.ModuleEndpoints.GetModules}/{id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<ModuleResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}