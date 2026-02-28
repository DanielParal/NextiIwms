using System.Net;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups;
using Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class DepositorGroupApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;

    public DepositorGroupApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllCrudEndpoints_ShouldCreateUpdateAndDeleteDepositorGroup_WhenCallingAllEndpointsOneByOne()
    {
        const string code = "DepositorGroupApiTests_DepositorGroupCode";
        const string nameWhenCreated = "DepositorGroupApiTests_DepositorGroupName";
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = code,
                    Name = nameWhenCreated
                })
                .SendAndDeserializeAsync<DepositorGroupResponse>();
        
        await VerifyDepositorGroupAsync(createdResponse.responseContent!.Code, nameWhenCreated);
        
        var updateBody = new
        {
            Name = "DepositorGroupApiTests_DepositorGroupNameUpdated"
        };
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups}/{createdResponse.responseContent!.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyDepositorGroupAsync(createdResponse.responseContent!.Code, updateBody.Name);
        
        await new HttpRequestBuilder(_client, HttpMethod.Delete,
                    $"{SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups}/{createdResponse.responseContent.Code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();

        await VerifyNotFoundDepositorGroupAsync(createdResponse.responseContent!.Code);
    }
    
    [Fact]
    public async Task CreateDepositorGroup_ShouldFail_WhenPassingEmptyCode()
    {
        var createdResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = string.Empty,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateSameDepositorGroups_ShouldSecondFail_WhenPassingSameCodes()
    {
        var sameCode = $"SameCode_{Guid.NewGuid()}";
        await new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        var secondCreatedResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Post, SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(new
                {
                    Code = sameCode,
                    Name = Guid.Empty
                })
                .SendAsync();
        
        secondCreatedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task DeleteDepositorGroup_ShouldFail_WhenCodeStillUsedInDepositor()
    {
        var deletedResponse = await new HttpRequestBuilder(_client, HttpMethod.Delete,
                $"{SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups}/{_fixture.ApiSeedData.DepositorGroupCode}")
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        deletedResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        deletedResponse.responseContent!.Errors.First().Slug.ShouldBe(DepositorGroupErrors.ValidationCodeIsUsedInDepositors(_fixture.ApiSeedData.DepositorCode).Code);
    }

    private async Task VerifyDepositorGroupAsync(string code, string expectedName)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorGroupResponse>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        getResponse.responseContent.Code.ShouldBe(code);
        getResponse.responseContent.Name.ShouldBe(expectedName);
    }
    
    private async Task VerifyNotFoundDepositorGroupAsync(string code)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups}/{code}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<DepositorGroupResponse>(ensureSuccessStatusCode: false);
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}