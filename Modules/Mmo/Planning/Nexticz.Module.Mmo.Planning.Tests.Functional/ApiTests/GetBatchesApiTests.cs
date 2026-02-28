using System.Net;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Planning.Presentation;
using Nexticz.Module.Mmo.SharedTesting;
using Shouldly;

namespace Nexticz.Module.Mmo.Planning.Tests.Functional.ApiTests;

[Collection(nameof(PlanningApiCollection))]
public class GetBatchesApiTests(PlanningApiFactoryFixture fixture)
{
    private readonly HttpClient _client = fixture.Factory.CreateClient();
    
    [Fact]
    public async Task GetBatches_ShouldReturnOrderedWashingMachines_WhenGettingAllWashingMachines()
    {
        var getResponse = await
            new HttpRequestBuilder(_client, HttpMethod.Get, PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)
                .WithXApiKeyHeader(UsersDbSeeder.XApiKeyForkLiftLoaderMember)
                .SendAndDeserializeAsync<WashingMachineResponse[]>();
        
        var washingMachineCodes = getResponse.responseContent!.Select(x => x.Code).ToArray();
        
        getResponse.responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        getResponse.responseContent.ShouldNotBeNull();
        washingMachineCodes[0].ShouldBe("MYCKA_1");
        washingMachineCodes[1].ShouldBe("MYCKA_2");
        washingMachineCodes[2].ShouldBe("MYCKA_3");
        washingMachineCodes[3].ShouldBe("MYCKA_4");
        washingMachineCodes[4].ShouldBe("MYCKA_5");
        washingMachineCodes[5].ShouldBe("MYCKA_6");
        washingMachineCodes[6].ShouldBe("MYCKA_7");
    }
}