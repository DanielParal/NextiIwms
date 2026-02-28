using MediatR;
using Moq;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines.Queries.GetFilteredWashingMachineResponsesByPackagingCode;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines.Queries;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Unit.Application.WashingMachines;

public class GetFilteredWashingMachineResponsesByPackagingCodeQueryHandlerTests
{
    private readonly Mock<ISender> _mockSender = new();
    private const string WashingMachineCode1 = "WM1";
    private const string WashingMachineCode2 = "WM2";
    private const string WashingMachineCode3 = "WM3";

    [Fact]
    public async Task Handle_ShouldReturnWashingMachineCodes_WhenValidPackagingCodeProvided()
    {
        // Arrange
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery("ValidCode", null);
        var packagingResponse = CreatePackagingResponse(180, 200);

        var washingMachineResponse = new[]
        {
            CreateWashingMachineResponse(WashingMachineCode1, 150, 500, 400),
            CreateWashingMachineResponse(WashingMachineCode2, 180, 500, 450)
        };

        _mockSender.Setup(x => x.Send(It.IsAny<GetPackagingResponseByCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(washingMachineResponse);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldContain(x => x.Code == WashingMachineCode1 || x.Code == WashingMachineCode2);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyArray_WhenNoMatchingWashingMachines()
    {
        // Arrange
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery("ValidCode", null);
        var packagingResponse = CreatePackagingResponse(180, 200);

        var emptyWashingMachines = Array.Empty<WashingMachineResponse>();

        _mockSender.Setup(x => x.Send(It.IsAny<GetPackagingResponseByCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyWashingMachines);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnIntersectingWashingMachineCodes_WhenSisterPackagingCodeProvided()
    {
        // Arrange
        const string validCode1 = "ValidCode1";
        const string validSisterCode1 = "ValidSisterCode1";
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery(validCode1, validSisterCode1);
        var packagingResponse1 = CreatePackagingResponse(180, 200);

        var packagingResponse2 = CreatePackagingResponse(
            180, 
            200, 
            [ new WashingMachineSpeedContract(WashingMachineCode2, WashingMachineSpeedLevelContract.Speed2) ]);

        var washingMachineResponse = new[]
        {
            CreateWashingMachineResponse(WashingMachineCode1, 150, 500, 400),
            CreateWashingMachineResponse(WashingMachineCode2, 180, 500, 450)
        };
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                codeQuery => codeQuery.Code == validCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse1);
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                sisterCodeQuery => sisterCodeQuery.Code == validSisterCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse2);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(washingMachineResponse);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldContain(x => x.Code == WashingMachineCode2);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyArray_WhenPackagingCodeIsInvalid()
    {
        // Arrange
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery("InvalidCode", null);

        _mockSender.Setup(x => x.Send(It.IsAny<GetPackagingResponseByCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WashingMachineErrors.NotFoundWashingMachineWithCode);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnOnlyFirstWashingMachine_WhenDepthIsSmallerHigherThanWashingMachineHeight()
    {
        // Arrange
        const string validCode1 = "ValidCode1";
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery(validCode1, null);
        var packagingResponse1 = CreatePackagingResponse(480, 200);
        

        var washingMachineResponse = new[]
        {
            CreateWashingMachineResponse(WashingMachineCode1, 150, 500, 500),
            CreateWashingMachineResponse(WashingMachineCode2, 180, 500, 400)
        };
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                codeQuery => codeQuery.Code == validCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse1);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(washingMachineResponse);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldContain(x => x.Code == WashingMachineCode1);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNoWashingMachines_WhenAllWashingMachinesAreNotWorking()
    {
        // Arrange
        const string validCode1 = "ValidCode1";
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery(validCode1, null);
        var packagingResponse1 = CreatePackagingResponse(200, 200);
        

        var washingMachineResponse = new[]
        {
            CreateWashingMachineResponse(WashingMachineCode1, 150, 500, 500, 2, WashingMachineStatusContract.Eliminated),
            CreateWashingMachineResponse(WashingMachineCode2, 180, 500, 400, 2, WashingMachineStatusContract.Maintenance)
        };
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                codeQuery => codeQuery.Code == validCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse1);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(washingMachineResponse);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe([]);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnOnlyFirst_WhenSecondAndThirdDoesNotFitWithWidth()
    {
        // Arrange
        const string validCode1 = "ValidCode1";
        const string validSisterCode1 = "ValidSisterCode1";
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery(validCode1, validSisterCode1);
        var packagingResponse1 = CreatePackagingResponse(100, 20);
        var packagingResponse2 = CreatePackagingResponse(200, 2000);

        var washingMachineResponse = new[]
        {
            CreateWashingMachineResponse(WashingMachineCode1, 10, 3000, 400),
            CreateWashingMachineResponse(WashingMachineCode2, 180, 500, 450),
            CreateWashingMachineResponse(WashingMachineCode3, 30, 1000, 450)
        };
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                codeQuery => codeQuery.Code == validCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse1);
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                sisterCodeQuery => sisterCodeQuery.Code == validSisterCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse2);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(washingMachineResponse);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldContain(x => x.Code == WashingMachineCode1);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnOnlyFirst_WhenSecondAndThirdAreSingleLines()
    {
        // Arrange
        const string validCode1 = "ValidCode1";
        const string validSisterCode1 = "ValidSisterCode1";
        var query = new GetFilteredWashingMachineResponsesByPackagingCodeQuery(validCode1, validSisterCode1);
        var packagingResponse1 = CreatePackagingResponse(100, 200);
        var packagingResponse2 = CreatePackagingResponse(200, 220);

        var washingMachineResponse = new[]
        {
            CreateWashingMachineResponse(WashingMachineCode1, 10, 3000, 400),
            CreateWashingMachineResponse(WashingMachineCode2, 180, 500, 450, numberOfLines: 1),
            CreateWashingMachineResponse(WashingMachineCode3, 30, 1000, 450, numberOfLines: 1)
        };
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                codeQuery => codeQuery.Code == validCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse1);
        
        _mockSender.Setup(x => x.Send(It.Is<GetPackagingResponseByCodeQuery>(
                sisterCodeQuery => sisterCodeQuery.Code == validSisterCode1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(packagingResponse2);

        _mockSender.Setup(x => x.Send(It.IsAny<GetWashingMachineResponsesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(washingMachineResponse);

        // Act
        var result = await CreateSut().Handle(query, CancellationToken.None);

        // Assert
        result.ShouldContain(x => x.Code == WashingMachineCode1);
    }
    
    private GetFilteredWashingMachineResponsesByPackagingCodeQueryHandler CreateSut() => new(_mockSender.Object);

    private static PackagingResponse CreatePackagingResponse(int depth, int height, WashingMachineSpeedContract[]? speeds = null)
    {
        return new PackagingResponse(
            Guid.NewGuid(), "ValidCode", "TypeA", "Depositor", "CirculationA", "12345", "PackageA", true,
            new DimensionsContract(depth, 300, height), 200, 
            speeds ?? [
                new WashingMachineSpeedContract(WashingMachineCode1, WashingMachineSpeedLevelContract.Speed2),
                new WashingMachineSpeedContract(WashingMachineCode2, WashingMachineSpeedLevelContract.Speed2),
                new WashingMachineSpeedContract(WashingMachineCode3, WashingMachineSpeedLevelContract.Speed2)
            ]);
    }
    
    private static WashingMachineResponse CreateWashingMachineResponse(
        string code, int minWidth, int maxWidth, int maxHeight, int numberOfLines = 2, WashingMachineStatusContract status = WashingMachineStatusContract.Working)
    {
        return new WashingMachineResponse(
            Guid.NewGuid(), 
            code, "Note1", status, 250, minWidth, maxWidth, maxHeight, 90, 70, numberOfLines,
            50, 70, 100,
            Enumerable.Range(1, numberOfLines)
                .Select(lineNumber => new WashingMachineLineContract($"Line{lineNumber}", true, new PrinterSettingsContract(null, null, null, null, null)))
                .ToArray());
    }
}