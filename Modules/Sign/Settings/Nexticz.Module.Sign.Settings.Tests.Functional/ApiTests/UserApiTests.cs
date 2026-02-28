using System.Net;
using System.Text.Json;
using Nexticz.Module.Sign.Settings.Application.SigningDevices;
using Nexticz.Module.Sign.Settings.Application.Users;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.Settings.Presentation;
using Nexticz.Module.Sign.SharedTesting;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Errors.Models;
using Shouldly;

namespace Nexticz.Module.Sign.Settings.Tests.Functional.ApiTests;

[Collection(nameof(SettingsApiCollection))]
public class UserApiTests
{
    private readonly SettingsApiFactoryFixture _fixture;
    private readonly HttpClient _client;
    private const string AuthCreateAccountUrl = "/api/auth/accounts";

    public UserApiTests(SettingsApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Factory.CreateClient();
    }
    
    [Fact]
    public async Task HappyPathTestWithAllNotifications_ShouldCreateUpdateAndDeleteUser_WhenCallingAllEndpointsOneByOneInAuthModule()
    {
        var userName = $"MyUser_{Guid.NewGuid().ToString()}";
        var firstName = $"FirstName_{Guid.NewGuid().ToString()}";
        var lastName = $"LastName_{Guid.NewGuid().ToString()}";
        var roles = new[] {"SignMember"};
        var permissions = new[] {"SignManageAll"};
        
        var userId = await CreateUserInAuthAsync(userName, firstName, lastName, roles, permissions);
        await VerifyUserChangedFromAuthAsync(userName, $"{firstName} {lastName}", roles, permissions);
        
        var updatedPermissions = new[] {"SignManageAll", "SignManageSettings"};
        await UpdateUserInAuthAsync(userId, userName, firstName, lastName, roles, permissions);
        await VerifyUserChangedFromAuthAsync(userName, $"{firstName} {lastName}", roles, updatedPermissions);
        
        await DeleteUserInAuthAsync(userId);
        await VerifyNotFoundUserAsync(userName);
    }
    
    [Fact]
    public async Task UpdateUser_ShouldUpdateAllLists_WhenValidData()
    {
        var userName = $"MyUser_{Guid.NewGuid().ToString()}";
        var firstName = $"FirstName_{Guid.NewGuid().ToString()}";
        var lastName = $"LastName_{Guid.NewGuid().ToString()}";
        var roles = new[] {"SignMember"};
        var permissions = new[] {"SignManageAll"};
        
        await CreateUserInAuthAsync(userName, firstName, lastName, roles, permissions);

        var updateBody = new
        {
            DepositorCodes = Array.Empty<string>(),
            DepositorGroupCodes = new[] { _fixture.ApiSeedData.DepositorGroupCode },
            SigningDeviceCodes = new[] { _fixture.ApiSeedData.SigningDeviceCode },
            PrinterCodes = new[] { _fixture.ApiSeedData.PrinterCode1 },
            EmailSignature = string.Empty
        };
        
        await new HttpRequestBuilder(_client, HttpMethod.Put,
                    $"{SettingsEndpoints.UserEndpoints.GetUsers}/{userName}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(updateBody)
                .SendAsync();
        
        await VerifyUserAfterUpdateAsync(userName, 
            $"{firstName} {lastName}",
            updateBody.DepositorCodes,
            updateBody.DepositorGroupCodes,
            updateBody.SigningDeviceCodes);
    }
    
    [Fact]
    public async Task UpdateUser_ShouldFail_WhenNonExistingCode()
    {
        var userName = $"MyUser_{Guid.NewGuid().ToString()}";
        var firstName = $"FirstName_{Guid.NewGuid().ToString()}";
        var lastName = $"LastName_{Guid.NewGuid().ToString()}";
        var roles = new[] {"SignMember"};
        var permissions = new[] {"SignManageAll"};
        
        await CreateUserInAuthAsync(userName, firstName, lastName, roles, permissions);

        var updateBody = new
        {
            DepositorCodes = Array.Empty<string>(),
            DepositorGroupCodes = new[] { _fixture.ApiSeedData.DepositorGroupCode, "NonExistingCode" },
            SigningDeviceCodes = new[] { _fixture.ApiSeedData.SigningDeviceCode },
            PrinterCodes = new[] { _fixture.ApiSeedData.PrinterCode1 },
            EmailSignature = string.Empty
        };
        
        var updateResponse = await new HttpRequestBuilder(_client, HttpMethod.Put,
                $"{SettingsEndpoints.UserEndpoints.GetUsers}/{userName}")
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .WithContent(updateBody)
            .SendAndDeserializeAsync<ApiErrorResponse>(ensureSuccessStatusCode: false);
        
        updateResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        updateResponse.responseContent!.GetType().ShouldBe(typeof(ApiErrorResponse));
        updateResponse.responseContent!.Errors.ShouldContain(x => x.Slug == UserErrors.ValidationDidNotFindAllDepositorGroups.Code);
        updateResponse.responseContent!.Errors.ShouldContain(x => x.Message == UserErrors.ValidationDidNotFindAllDepositorGroups.Description);
    }

    private async Task<string> CreateUserInAuthAsync(string userName, string firstName, string lastName, string[] roles, string[] permissions)
    {
        await
            new HttpRequestBuilder(_client, HttpMethod.Post, AuthCreateAccountUrl)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateObjectForAuthRequest(userName, firstName, lastName, roles, permissions))
                .SendAsync();
        
        var userResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, $"{AuthCreateAccountUrl}/{userName}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAsync();
        
        var responseJsonContent = await userResponse.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(responseJsonContent);
        return doc.RootElement.GetProperty("id").GetString()!;
    }
    
    private async Task UpdateUserInAuthAsync(string id, string userName, string firstName, string lastName, string[] roles, string[] permissions)
    {
        await new HttpRequestBuilder(_client, HttpMethod.Put, $"{AuthCreateAccountUrl}/{id}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .WithContent(
                    GenerateObjectForAuthRequest(userName, firstName, lastName, roles, permissions))
                .SendAsync();
    }
    
    private async Task DeleteUserInAuthAsync(string id)
    {
        await new HttpRequestBuilder(_client, HttpMethod.Delete, $"{AuthCreateAccountUrl}/{id}")
            .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
            .SendAsync();
    }

    private static object GenerateObjectForAuthRequest(string userName, string firstName, string lastName, string[] roles, string[] permissions)
    {
        return new
        {
            Username = userName,
            Password = "SecurePwd123!",
            Firstname = firstName,
            Lastname = lastName,
            Company = Guid.NewGuid(),
            Email = $"{Guid.NewGuid()}@gmail.com",
            PhoneNumber = string.Empty,
            BlockedFrom = (DateTime?)null,
            DefaultUrl = string.Empty,
            Roles = roles,
            Permissions = permissions,
        };
    }
    
    private async Task VerifyUserChangedFromAuthAsync(string userName, string userFullName, string[] roles, string[] permissions)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.UserEndpoints.GetUsers}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<UserResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        
        var user = getResponse.responseContent.Data.FirstOrDefault(x => x.UserName == userName);
        user.ShouldNotBeNull();
        user.UserName.ShouldBe(userName);
        user.FullName.ShouldBe(userFullName);
        foreach (var roleContract in user.Roles)
        {
            roles.ShouldContain(roleContract.ToString());
        }

        foreach (var permissionContract in user.Permissions)
        {
            permissions.ShouldContain(permissionContract.ToString());
        }
    }
    
    private async Task VerifyUserAfterUpdateAsync(string userName, string fullName, string[] depositorCodes, 
        string[] depositorGroupCodes, string[] signingDeviceCodes)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.UserEndpoints.GetUsers}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<UserResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        
        var user = getResponse.responseContent.Data.FirstOrDefault(x => x.UserName == userName);
        user.ShouldNotBeNull();
        user.UserName.ShouldBe(userName);
        user.DepositorCodes.ShouldBeEquivalentTo(depositorCodes);
        user.DepositorGroupCodes.ShouldBeEquivalentTo(depositorGroupCodes);
        user.SigningDeviceCodes.ShouldBeEquivalentTo(signingDeviceCodes);
        user.FullName.ShouldBeEquivalentTo(fullName);
    }
    
    private async Task VerifyNotFoundUserAsync(string userName)
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, 
                    $"{SettingsEndpoints.UserEndpoints.GetUsers}")
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyDeveloper)
                .SendAndDeserializeAsync<FilteredResult<UserResponse>>();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        var user = getResponse.responseContent.Data.FirstOrDefault(x => x.UserName == userName);
        user.ShouldBeNull();
    }
}