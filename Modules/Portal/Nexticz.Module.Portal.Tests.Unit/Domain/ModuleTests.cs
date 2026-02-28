using Nexticz.Module.Portal.Domain.ModuleAggregate;
using Shouldly;

namespace Nexticz.Module.Portal.Tests.Unit.Domain;

public class ModuleTests
{
    private readonly DateTimeOffset _utcNow = new(2025, 10, 22, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateFrom_WithValidData_ShouldCreateModule()
    {
        var name = "Test Module";
        var icon = "test-icon";
        var baseUrl = "http://test.com";
        var isActive = true;
        var sortOrder = 1;

        var result = Portal.Domain.ModuleAggregate.Module.CreateFrom(name, icon, baseUrl, isActive, sortOrder, _utcNow);

        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe(name);
        result.Value.Icon.ShouldBe(icon);
        result.Value.BaseUrl.ShouldBe(baseUrl);
        result.Value.IsActive.ShouldBe(isActive);
        result.Value.SortOrder.ShouldBe(sortOrder);
        result.Value.CreatedAt.ShouldBe(_utcNow);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateFrom_WithInvalidName_ShouldReturnError(string invalidName)
    {
        var icon = "test-icon";
        var baseUrl = "http://test.com";
        var isActive = true;
        var sortOrder = 1;

        var result = Portal.Domain.ModuleAggregate.Module.CreateFrom(invalidName, icon, baseUrl, isActive, sortOrder, _utcNow);

        result.IsError.ShouldBeTrue();
    }
    
    [Fact]
    public void Update_WithValidData_ShouldUpdateModule()
    {
        var module = Portal.Domain.ModuleAggregate.Module.CreateFrom("Original Name", "original-icon", "http://original.com", true, 1, _utcNow).Value;
        var newName = "Updated Name";
        var newIcon = "updated-icon";
        var newBaseUrl = "http://updated.com";
        var newIsActive = false;

        var result = module.Update(newName, newIcon, newBaseUrl, newIsActive);

        result.IsError.ShouldBeFalse();
        module.Name.ShouldBe(newName);
        module.Icon.ShouldBe(newIcon);
        module.BaseUrl.ShouldBe(newBaseUrl);
        module.IsActive.ShouldBe(newIsActive);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Update_WithInvalidName_ShouldReturnError(string invalidName)
    {
        var module = Portal.Domain.ModuleAggregate.Module.CreateFrom("Original Name", "original-icon", "http://original.com", true, 1, _utcNow).Value;
        var newIcon = "updated-icon";
        var newBaseUrl = "http://updated.com";
        var newIsActive = false;

        var result = module.Update(invalidName, newIcon, newBaseUrl, newIsActive);

        result.IsError.ShouldBeTrue();
    }
}